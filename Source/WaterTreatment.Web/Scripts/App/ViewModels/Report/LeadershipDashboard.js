'use strict';

define(['knockout', 'jquery', 'webService', 'moment', 'googleCharts', 'bootstrap', 'bootstrap-multiselect', 'datePicker'], function (ko, $, webService, moment) {
    return function (dashboard, complience) {
        var self = this;
        self.Dashboard = ko.observable(dashboard);
        self.complience = ko.observable(complience);
        self.isProcessingRequest = ko.observable(false);
        self.showNoDataMessage = ko.observable(false);
        self.selectedGroup = ko.observable();
        self.sites = ko.observableArray();

        self.inRange = ko.observable(dashboard.InRange);
        self.allMeasured = ko.observable(dashboard.SomeOutofRange);
        self.missedMeasurements = ko.observable(dashboard.Missedmeasurements);
        self.noMeasurements = ko.observable(dashboard.Nomeasurements);

        self.selectedSite = ko.observable(self.complience()[0]);

        self.NotMeasuredSites = ko.observableArray();



        google.charts.load('current', { 'packages': ['corechart'] });
        google.charts.setOnLoadCallback(drawChart);

        self.onChange = function () {
            window.location.href = '/Reports/SiteDashboard?site=' + self.selectedSite(); //relative to domain
            //alert("Hello " + self.selectedSite());
        };

        self.close = function () {
            self.isProcessingRequest(false);
            $("#siteModal").modal('hide');
        }

        function drawChart() {
            var siteData = '';

            var data = google.visualization.arrayToDataTable([
              ['Complience', 'Percent Bases'],
              ['In Range', self.inRange()],
              ['Some Out of Range', self.allMeasured()],
              ['Missed measurements', self.missedMeasurements()]
            ]);

            var options = {
                title: 'Measurement complience by Sites',
                colors: ['#32cd32', '#ffd700', '#ff0000', '#f3b49f', '#f6c7b6']
            };

            var chart = new google.visualization.PieChart(document.getElementById('piechart'));

            chart.draw(data, options);

            google.visualization.events.addListener(chart, 'select', selectHandler);


            function selectHandler() {
                var selectedItem = chart.getSelection()[0];
                self.isProcessingRequest(false);
                if (selectedItem) {
                    var topping = data.getValue(selectedItem.row, 0);
                    self.selectedGroup('(' + topping + ')...');
                    //alert('The user selected ' + topping);
                    var siteData = self.complience();

                    var site = function (name, id) {
                        this.siteName = name;
                        this.siteId = id;
                    };

                    if (topping == 'In Range') {
                        var rows = [];
                        Object.keys(siteData).forEach(function (siteId) {
                            if (siteData[siteId].AllInRange == 1) {
                                var siteDatum = siteData[siteId];
                                var row = new site(siteDatum.siteName, siteDatum.siteId);
                                rows.push(row);
                            }
                        })
                    }
                    else if (topping == 'Some Out of Range') {
                        var rows = [];
                        Object.keys(siteData).forEach(function (siteId) {
                            if (siteData[siteId].AllInRange == 0 && siteData[siteId].AllMeasured == 1) {
                                var siteDatum = siteData[siteId];
                                var row = new site(siteDatum.siteName, siteDatum.siteId);
                                rows.push(row);
                            }
                        })
                    }
                    else if (topping == 'Missed measurements') {
                        var rows = [];
                        Object.keys(siteData).forEach(function (siteId) {
                            if (siteData[siteId].AllInRange == 0 && siteData[siteId].AllMeasured == 0 && siteData[siteId].SomeMeasured != null) {
                                var siteDatum = siteData[siteId];
                                var row = new site(siteDatum.siteName, siteDatum.siteId);
                                rows.push(row);
                            }
                        })
                    }

                    var rows1 = [];
                    Object.keys(siteData).forEach(function (siteId) {
                        if (siteData[siteId].AllInRange == 0 && siteData[siteId].AllMeasured == 0 && siteData[siteId].SomeMeasured == null) {
                            var siteDatum = siteData[siteId];
                            var row = new site(siteDatum.siteName, siteDatum.siteId);
                            rows1.push(row);
                        }
                    })

                    self.sites(rows);
                    self.NotMeasuredSites(rows1);
                    $("#siteModal").modal('show');
                }
            }

        }

    }
})