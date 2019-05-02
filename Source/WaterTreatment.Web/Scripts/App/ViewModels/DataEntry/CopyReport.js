'use strict';
define(['knockout', 'pager', 'webService', 'moment', 'util', 'modal', 'datePicker'], function (ko, pager, webService, moment, util, modal) {
    var vm = function (sites, initialState) {
        var self = this;
        var dateFormat = 'M/D/YYYY';

        self.reports = ko.observableArray();
        self.sites = ko.observable(sites);
        self.showCreatedBy = ko.observable();
        self.showAdminActions = ko.observable();
        self.successMessage = ko.observable('');
        self.statuses = ko.observable([
            { Name: 'Draft', Value: 'draft' },
            { Name: 'Submitted', Value: 'submitted' }
        ]);

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

        self.filters = {
            Sites: ko.observable().extend({ rateLimit: { method: 'notifyWhenChangesStop' } }),
            MeasurementDateStart: ko.observable().extend({ rateLimit: { method: 'notifyWhenChangesStop', timeout: 400 } }),
            MeasurementDateEnd: ko.observable().extend({ rateLimit: { method: 'notifyWhenChangesStop', timeout: 400 } })
        };

        self.subSite = ko.observable();
        self.subscriptions = ko.observableArray([]);
        self.isProcessingSubAction = ko.observable(false);

        if (initialState) {
            self.showCreatedBy(initialState.ShowCreatedBy);
            self.showAdminActions(initialState.ShowAdminActions);
            self.subscriptions(initialState.Subscriptions);
            self.filters.MeasurementDateStart(initialState.Filters.MeasurementDateStart);
            self.filters.MeasurementDateEnd(initialState.Filters.MeasurementDateEnd);

            if (initialState.Filters.Site) {
                self.filters.Sites(initialState.Filters.Site);
                self.subSite(initialState.Filters.Site[0]);
            }
        }

        var search = function (payload) {
            self.isProcessingRequest(true);
            // The datepicker gives us midnight local time, but we're interested in midnight UTC since that's how it's getting stored in the database
            // when you pick a generic (timestamp-less) Measurement date for your report, so reconstruct a UTC date using the same year, month, and date
            if (payload.Filters.MeasurementDateStart != null) {
                var measurementDateStart = moment(payload.Filters.MeasurementDateStart);
                payload.Filters.MeasurementDateStart = moment.utc(measurementDateStart.years() + '-' + (measurementDateStart.months() + 1) + '-' + measurementDateStart.date(), 'YYYY-MM-DD');
            }

            if (payload.Filters.MeasurementDateEnd != null) {
                var measurementDateEnd = moment(payload.Filters.MeasurementDateEnd);
                payload.Filters.MeasurementDateEnd = moment.utc(measurementDateEnd.years() + '-' + (measurementDateEnd.months() + 1) + '-' + measurementDateEnd.date(), 'YYYY-MM-DD');
            }

            return webService.Post('/Reports/SearchReports/', payload).then(function (data) {
                data.Results.forEach(function (d) {
                    d.StartedOn = moment.utc(d.StartedOn).local().format(dateFormat);
                    d.SubmittedOn = d.SubmittedOn != null ? moment.utc(d.SubmittedOn).local().format(dateFormat) : null;
                });
                self.isProcessingRequest(false);
                return data;
            });
        };

        self.pager = new pager(search, self.filters, self.reports);

        self.modal = new modal();
        //self.modal.comments = ko.observable().extend({ rateLimit: { method: 'notifyWhenChangesStop', timeout: 400 } });

        self.copyReport = function (report) {
            //if (self.isProcessingRequest()) return;
            self.isProcessingRequest(true);
            self.successMessage('Coping...');
            //self.pager.waitingForServer(true);
            var payload = { ReportId: report.Id };
            webService.Post('/DataEntry/CopyReport/' + report.Id).then(function (data) {
                self.isProcessingRequest(false);
                self.successMessage("The report was successfully copied to a New Report. Please, go to Data entry page to see the copied report.");
            }).done();
        };

        //self.setModalReport = function (report) {
        //    self.modalReport(report);
        //};


    };

    return vm;
})