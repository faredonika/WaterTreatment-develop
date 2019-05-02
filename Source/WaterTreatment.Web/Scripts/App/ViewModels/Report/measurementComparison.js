'use strict';
define(['knockout', 'jquery', 'webService', 'moment', 'q', 'bootstrap', 'bootstrap-multiselect', 'googleCharts', 'datePicker'], function (ko, $, webService, moment, Q) {
    return function (parameters, sitesByParameter) {
        var self = this;
        google.charts.load('current', { packages: ['corechart', 'bar'] });

        self.parameters = ko.observable(parameters);

        self.sites = ko.observable();
        self.selectedParameterId = ko.observable().extend({ rateLimit: { timeout: 500, method: "notifyWhenChangesStop" } });
        self.selectedSites = ko.observable().extend({ rateLimit: { timeout: 500, method: "notifyWhenChangesStop" } });
        self.submitFrom = ko.observable();
        self.submitTo = ko.observable();

        self.selectedParameterId.subscribe(function (paramId) {
            self.sites(sitesByParameter[paramId] || []);
        });

        self.missingLatestValue = ko.observable([]);
        self.systemType = ko.observable();

        self.isProcessingRequest = ko.observable(false);
        self.showNoDataMessage = ko.observable(false);

        self.msOptions = {
            maxHeight: 700,
            includeSelectAllOption: true,
            enableFiltering: true,
            filterBehavior: 'text',
            enableCaseInsensitiveFiltering: true,
            nonSelectedText: 'Any',
            buttonWidth: '500px'
        };

        self.measurementMetaData = ko.observable({});

        ko.computed(function () {
            var dateFormat = 'M/D/YYYY';
            var parameterId = self.selectedParameterId();
            var sites = self.selectedSites();
            var from = moment(self.submitFrom()).format(dateFormat);
            var to = moment(self.submitTo()).format(dateFormat);

            if (parameterId) {
                self.isProcessingRequest(true);
                self.showNoDataMessage(false);
                webService.Post('/Reports/GetMeasurementComparison', { ParameterId: parameterId, SiteIds: sites, From: from, To: to }).then(function (data) {
                    var modelData = JSON.parse(data);
                    self.measurementMetaData(modelData.MeasurementComparisonData);

                    var systemType = modelData.SystemType;
                    var parameter = modelData.Parameter;
                    var parameterType = modelData.ParameterType;
                    var units = modelData.Units;

                    var headers = [];
                    var rows = [];
                    var row1 = [];
                    var atLeastOneMeasurement = false;
                    var hasInvalidColor = false;
                    var firstTime = true;
                    var count = 0;

                    var order = {};
                    var colors = { 1: 'Light', 2: 'Medium', 3: 'Dark' };
                    for (var uniqId in modelData.Reports) {
                        //if (modelData.ComparisonData.hasOwnProperty(uniqId)) {
                        var header = modelData.Reports[uniqId];
                        row1.push(header);
                    }
                    headers.push(row1);
                    //rows.push(row1);

                    for (var dataId in modelData.MeasurementComparisonData) {
                        var row = [];
                        var paramCompData = modelData.MeasurementComparisonData[dataId];
                        var site = paramCompData.SiteName + ' \n' + paramCompData.BuildingName + ' \n' + paramCompData.Location;
                        var min = paramCompData.Min;
                        var max = paramCompData.Max;
                        row.push(site);
                            count = 2;
                            for (var uniqId in paramCompData.Values) {
                                if (paramCompData.Values.hasOwnProperty(uniqId)) {

                                    var dataValue = paramCompData.Values[uniqId];
                                    row.push(dataValue);
                                }
                            }
                            atLeastOneMeasurement = true;
                            rows.push(row);
                            count++
                    };

                    
                    if (atLeastOneMeasurement) {

                        //rows, units, systemType, headers,
                        drawChart(rows, modelData.Units, systemType, parameter, headers).then(function () {
                            self.systemType(modelData.SystemType);
                            //self.missingLatestValue(modelData.MissingMeasurements);
                            self.isProcessingRequest(false);
                        }).done();
                        self.isProcessingRequest(false);
                    } else {
                        self.systemType(modelData.SystemType);
                        //self.missingLatestValue(modelData.MissingMeasurements);
                        self.isProcessingRequest(false);
                        self.showNoDataMessage(true);
                    }
                }).done();
            }
        });

        function drawChart(rows, units, SystemType, parameter, headers) {
            var deferred = Q.defer();       // the draw() function is asynchronous, so we'll return a promise that resolves when the chart is done rendering

            var dt = new google.visualization.DataTable();
            dt.addColumn('string', 'Sites');
            dt.addColumn('number', 'Min');
            dt.addColumn('number', 'Max');

            for (var i = 3; i < headers[0].length; i++) {
                    dt.addColumn('number', headers[0][i]);
            }

            dt.addRows(rows);

            var font = 'Arial';
            var fontSize = 14;
            var textStyleArial = {
                textStyle: {
                    fontName: 'Arial', fontSize: fontSize
                }
            };

            var options = {
                height: 800,
                width: 950,
                chartArea: { left: 70, top: 50, width: '80%', height: '75%' },
                title: parameter + ' - ' + SystemType,
                vAxis: { title: units },
                hAxis: {
                    title: 'Sites',
                    direction: -1,
                    slantedText: true,
                    slantedTextAngle: 60,
                    fontSize: 7
                },
                legend: {
                    position: 'top',
                    maxLines: 1
                },
                seriesType: 'bars',
                //series: {5: {type: 'line'}}

                series: {
                    0: {
                        type: 'line'
                    },
                    1: {
                        type: 'line'
                    }
                }

            };

            google.charts.load('current', { 'packages': ['corechart'] });
            var chart = new google.visualization.ComboChart(document.getElementById('measurement-bar-chart'));
            chart.draw(dt, options);

            return deferred.promise;
        };
    };
});