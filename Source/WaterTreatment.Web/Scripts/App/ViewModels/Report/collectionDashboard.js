'use strict';
define(['knockout', 'jquery', 'webService', 'q', 'moment', 'googleCharts', 'bootstrap', 'bootstrap-multiselect', 'datePicker'], function (ko, $, webService, Q, moment) {
    return function (sites) {
        var self = this;
        google.charts.load('current', { packages: ['bar'] });

        self.sites = ko.observable(sites);
        self.selectedSites = ko.observable().extend({ rateLimit: { timeout: 1000, method: "notifyWhenChangesStop" } });
        self.submitFrom = ko.observable();
        self.submitTo = ko.observable();
        self.isProcessingRequest = ko.observable(false);
        self.showNoDataMessage = ko.observable(false);

        self.msOptions = {
            maxHeight: 400,
            includeSelectAllOption: true,
            enableFiltering: true,
            filterBehavior: 'text',
            enableCaseInsensitiveFiltering: true,
            nonSelectedText: 'Any',
            buttonWidth: '350px'
        };

        self.update = function () {
            var dateFormat = 'M/D/YYYY';

            var sites = self.selectedSites(),
                from = moment(self.submitFrom()).format(dateFormat),
                to = moment(self.submitTo()).format(dateFormat);

            self.isProcessingRequest(true);
            webService.Post('/Reports/GetCollectionData', { SiteIds: sites, From: moment(self.submitFrom()).format(dateFormat), To: moment(self.submitTo()).format(dateFormat) }).then(function (data) {
                var d = JSON.parse(data);
                var siteData = d.SiteCollectionData;

                // Figure out what axis max should be; we have to set this explicitly to get the two series to scale properly
                var maxParams = 0;
                var rows = [];
                Object.keys(siteData).forEach(function (siteId) {
                    var row = [];
                    var siteDatum = siteData[siteId];
                    row.push(siteDatum.Name);
                    row.push(siteDatum.InBounds);
                    row.push(siteDatum.OutOfBounds);
                    row.push(siteDatum.NotCollected);
                    row.push(siteDatum.Late);
                    rows.push(row);

                    var paramsCount = siteDatum.InBounds + siteDatum.OutOfBounds + siteDatum.NotCollected + siteDatum.Late;
                    if (paramsCount > maxParams) {
                        maxParams = paramsCount;
                    }
                });


                if (rows.length > 0) {
                    self.showNoDataMessage(false);
                    drawChart(rows, maxParams).then(function () {
                        self.isProcessingRequest(false);
                    }).done();
                }
                else {
                    self.showNoDataMessage(true);
                    self.isProcessingRequest(false);
                }

            }).done();
        };

        self.selectedSites.subscribe(self.update);
        self.submitFrom.subscribe(self.update);
        self.submitTo.subscribe(self.update);
        self.selectedSites([]);

        function drawChart(rows, maxParams) {
            var deferred = Q.defer();       // the draw() function is asynchronous, so we'll return a promise that resolves when the chart is done rendering
            
            var dt = new google.visualization.DataTable();
            dt.addColumn('string', 'Sites');
            dt.addColumn('number', 'In Range');
            dt.addColumn('number', 'In Range 10%');
            dt.addColumn('number', 'Out of Range');
            dt.addColumn('number', 'Not Measured');
            
            dt.addRows(rows);

            var font = 'Arial';
            var fontSize = 14;
            var textStyleArial = {
                textStyle: {
                    fontName: 'Arial', fontSize: fontSize
                }
            };

            var heightParam = 250;
            if (rows.length > 1) {
                heightParam = 300+ rows.length * 50;
            }


            var options = {
                isStacked: true,
                chartArea: { width: '50%' },
                bars: 'horizontal',
                chart: {
                    title: 'Aggrigate number of Measurements',
                    fontSize: 18,
                    subtitle: ''
                },
                hAxis: {
                    title: 'Total Measurements',
                    minValue: 0,
                    textStyle: {
                        bold: true,
                        fontSize: 12,
                        color: '#d95f02'
                    },
                    titleTextStyle: {
                        bold: true,
                        fontSize: 18,
                        color: 'gray'
                    }
                },
                vAxis: textStyleArial,
                height: heightParam,
                series: {
                    0: { color: '#6f9654' },
                    1: { color: '#f1ca3a' },
                    2: { color: '#e21e41' },
                    3: { color: 'gray' }
                },
                tooltip: textStyleArial,
                legend: textStyleArial
            };

            var chart = new google.charts.Bar(document.getElementById('collection-bar-chart'));
            google.visualization.events.addListener(chart, 'ready', deferred.resolve);

            chart.draw(dt, google.charts.Bar.convertOptions(options));

            return deferred.promise;
        };

        
    };
});