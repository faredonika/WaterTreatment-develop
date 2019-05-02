'use strict';
define(['knockout', 'pager', 'webService', 'util', 'moment', 'App/Bindings/radioButtonGroup'], function (ko, Pager, webService, util, moment) {
    var IndexViewModel = function (sites) {
        var self = this;
        var dateFormat = 'M/D/YYYY';

        self.sites = sites;
        self.reports = ko.observableArray();

        self.msOptions = {
            maxHeight: 400,
            includeSelectAllOption: true,
            enableFiltering: true,
            filterBehavior: 'text',
            enableCaseInsensitiveFiltering: true,
            nonSelectedText: 'Any',
            buttonWidth: '350px'
        };
        self.isProcessingRequest = ko.observable(false);
        self.type = ko.observable('Both');

        self.filters = {
            Type: self.type
        };

        var endPoint = function (payload) {
            self.isProcessingRequest(true);
            return webService.Post('/DataEntry/GetInProgressReports/', payload).then(function (data) {
                data.Results.forEach(function (d) {
                    d.StartedOn = moment.utc(d.StartedOn).local().format(dateFormat);
                });
                self.isProcessingRequest(false);
                return data;
            });
        };

        self.pager = new Pager(endPoint, self.filters, self.reports);

        self.statusClass = function (report) {
            if (report.DaysElapsed > 7) {
                return 'danger';
            }
            return '';
        };

        self.delete = function (report) {
            if (self.isProcessingRequest()) return;
            self.isProcessingRequest(true);

            var message = 'To delete this report, please type the word \"delete\" and click OK.';
            util.promptForCommand(message, 'delete', function () {
                self.pager.waitingForServer(true);
                var payload = { ReportId: report.Id };
                webService.PostCSRF('/Reports/Delete/', payload, {}, 'html').then(function (data) {
                    self.pager.query();
                    $('#bannerContainer').html(data);
                }).done();
            }, function () {
                self.isProcessingRequest(false);
            });
        };
    };

    return IndexViewModel;
});