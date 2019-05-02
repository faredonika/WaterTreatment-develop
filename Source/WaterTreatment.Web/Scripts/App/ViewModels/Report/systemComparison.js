'use strict';
define(['knockout', 'jquery', 'webService', 'moment', 'bootstrap', 'bootstrap-multiselect', 'datePicker'], function (ko, $, webService, moment) {
    return function (systemTypes, sitesBySystemType) {
        var self = this;
        var dateFormat = 'M/D/YYYY';

        self.systemTypes = ko.observable(systemTypes);

        self.sites = ko.observable();
        self.selectedSystemTypeId = ko.observable().extend({ rateLimit: { timeout: 500, method: "notifyWhenChangesStop" } });
        self.selectedSites = ko.observable().extend({ rateLimit: { timeout: 500, method: "notifyWhenChangesStop" } });
        self.submitFrom = ko.observable();
        self.submitTo = ko.observable();
        self.isProcessingRequest = ko.observable(false);

        self.msOptions = {
            maxHeight: 400,
            includeSelectAllOption: true,
            enableFiltering: true,
            filterBehavior: 'text',
            enableCaseInsensitiveFiltering: true,
            nonSelectedText: 'Any',
            buttonWidth: '350px'
        };

        self.systemMetaData = ko.observable({});
        self.rows = ko.observable();

        self.order = ko.observable({});                     // systemId => position
        self.inverseOrder = ko.computed(function () {       // position => systemId
            var order = self.order();
            var inverseOrder = {};
            Object.keys(order).forEach(function (position) {
                inverseOrder[order[position]] = position;
            });
            return inverseOrder;
        });

        self.selectedSystemTypeId.subscribe(function (sysTypeId) {
            self.sites(sitesBySystemType[sysTypeId] || []);
        });


        ko.computed(function () {
            var systemTypeId = self.selectedSystemTypeId();
            var sites = self.selectedSites();
            if (systemTypeId) {
                self.isProcessingRequest(true);
                webService.Post('/Reports/GetSystemComparison/', { SystemTypeId: systemTypeId, SiteIds: sites, From: moment(self.submitFrom()).format(dateFormat), To: moment(self.submitTo()).format(dateFormat) }).then(function (data) {
                    var modelData = JSON.parse(data);
                    self.systemMetaData(modelData.SystemMetaData);

                    var rows = [];
                    var firstTime = true;
                    var order = {};
                    var colors = { 1: 'Light', 2: 'Medium', 3: 'Dark' };
                    for (var parameterId in modelData.ComparisonData) {
                        if (modelData.ComparisonData.hasOwnProperty(parameterId)) {
                            var row = {};
                            var paramData = modelData.ComparisonData[parameterId];
                            var paramMetaData = modelData.ParameterMetaData[parameterId];

                            var minBound = paramMetaData.MinBound;
                            var maxBound = paramMetaData.MaxBound;
                            if (paramMetaData.Type != 'Number') {
                                minBound = minBound != null ? colors[minBound] : null;
                                maxBound = maxBound != null ? colors[maxBound] : null;
                            }
                            row.MinBound = minBound;
                            row.MaxBound = maxBound;
                            row.ParameterName = paramMetaData.Name;
                            var count = 0;
                            for (var systemId in paramData) {
                                if (paramData.hasOwnProperty(systemId)) {
                                    if (firstTime) {
                                        order[systemId] = count;
                                    }
                                    var position = order[systemId];

                                    var measurementDatum = paramData[systemId];
                                    measurementDatum.SystemId = systemId;
                                    measurementDatum.ReportDate = measurementDatum.ReportDate ? moment(measurementDatum.ReportDate).format(dateFormat) : null;
                                    if (paramMetaData.Type != 'Number' && measurementDatum.Value != null) {
                                        measurementDatum.Value = colors[measurementDatum.Value];
                                    }
                                    row[position] = measurementDatum;

                                    count++;
                                }
                            }
                            rows.push(row);
                            firstTime = false;
                        }
                    }
                    self.order(order);

                    self.rows(rows);
                    self.systemMetaData.valueHasMutated();
                    self.isProcessingRequest(false);
                }).done();
            }
        });
    };
});