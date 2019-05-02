'use strict';
define(['knockout', 'jquery', 'webService', 'moment', 'bootstrap', 'bootstrap-multiselect', 'datePicker'], function (ko, $, webService, moment) {
    return function (sites, buildingsPerSite) {
        var self = this;
        var dateFormat = 'M/D/YYYY';
        self.sites = ko.observableArray(sites);
        self.selectedSite = ko.observable();

        self.buildingsPerSite = ko.observable(buildingsPerSite);
        self.buildings = ko.observable();
        self.selectedBuilding = ko.observable();
        self.submitFrom = ko.observable();
        self.submitTo = ko.observable();
        self.systems = ko.observable();

        self.isProcessingRequest = ko.observable(false);

        self.selectedSite.subscribe(function () {
            self.buildings(self.buildingsPerSite()[self.selectedSite()]);
            self.selectedBuilding.valueHasMutated();
        });

        self.update = function () {
            if (self.selectedSite()) {
                self.isProcessingRequest(true);
                webService.Get('/Reports/GetSiteReport/', { SiteId: self.selectedSite(), BuildingId: self.selectedBuilding() ? self.selectedBuilding() : null, From: moment(self.submitFrom()).format(dateFormat), To: moment(self.submitTo()).format(dateFormat) }).then(function (data) {
                    var systems = data;
                    systems.forEach(function (sys) {
                        sys.showChildren = ko.observable(false);
                        sys.OldestReportDate = sys.OldestReportDate ? moment(sys.OldestReportDate).format(dateFormat) : null;
                        sys.MostRecentReportDate = sys.MostRecentReportDate ? moment(sys.MostRecentReportDate).format(dateFormat) : null;
                        sys.DateRange = sys.OldestReportDate === sys.MostRecentReportDate ? sys.OldestReportDate : sys.OldestReportDate + ' - ' + sys.MostRecentReportDate;
                        var colors = { 1: 'Light', 2: 'Medium', 3: 'Dark' };
                        sys.Measurements.forEach(function (m) {
                            m.ReportDate = m.ReportDate ? moment(m.ReportDate).format(dateFormat) : null;
                            if (m.ParameterType != 'Number') {
                                m.MinBound = m.MinBound != null ? colors[m.MinBound] : null;
                                m.MaxBound = m.MaxBound != null ? colors[m.MaxBound] : null;
                                m.Value = m.Value != null ? colors[m.Value] : null;
                            }
                        });
                    });
                    self.systems(systems);
                    self.isProcessingRequest(false);
                }).done();
            }
        }

        self.selectedBuilding.subscribe(self.update);
        self.submitFrom.subscribe(self.update);
        self.submitTo.subscribe(self.update);

        self.toggleShowHistory = function (datum) {
            if (datum.Measurements.length > 0) {
                datum.showChildren(!datum.showChildren());
            }
        };

        self.buildDropdownEnabled = ko.computed(function () {
            return self.selectedSite() !== undefined;
        });
    };
});