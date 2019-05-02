'use strict';
define(['knockout', 'pager', 'webService', 'moment', 'util', 'modal', 'datePicker'], function (ko, pager, webService, moment, util, modal) {
    var vm = function (sites, initialState) {
        var self = this;
        var dateFormat = 'M/D/YYYY';

        self.reports = ko.observableArray();
        self.sites = ko.observable(sites);
        self.showCreatedBy = ko.observable();
        self.showAdminActions = ko.observable();
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
            MeasurementDateEnd: ko.observable().extend({ rateLimit: { method: 'notifyWhenChangesStop', timeout: 400 } }),
            ReportSubmitDate: ko.observable().extend({ rateLimit: { method: 'notifyWhenChangesStop', timeout: 400 } })
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
            self.filters.ReportSubmitDate(initialState.Filters.ReportSubmitDate);


            if (initialState.Filters.Site) {
                self.filters.Sites(initialState.Filters.Site);
                self.subSite(initialState.Filters.Site[0]);
            }
        }

        self.subOpsAllowed = ko.computed(function () {
            return !self.isProcessingRequest() && self.subSite() !== undefined;
        });

        self.isSubscribed = ko.computed(function () {
            return self.subscriptions().indexOf(self.subSite()) !== -1
        });

        self.subSubmitLabel = ko.computed(function () {
            return self.isSubscribed() ? 'Unsubscribe' : 'Subscribe';
        });

        //self.subscribe = function () {
        //    var payload = { id: self.subSite() };

        //    self.isProcessingRequest(true);
        //    self.isProcessingSubAction(true);

        //    webService.PostCSRF('/Reports/Subscribe/', payload, {}).then(function (subscriptions) {
        //        self.subscriptions(subscriptions);
        //        self.isProcessingSubAction(false);
        //        self.isProcessingRequest(false);
        //    });
        //}

        //self.unsubscribe = function () {
        //    var payload = { id: self.subSite() };

        //    self.isProcessingRequest(true);
        //    self.isProcessingSubAction(true);

        //    webService.PostCSRF('/Reports/Unsubscribe/', payload, {}).then(function (subscriptions) {
        //        self.subscriptions(subscriptions);
        //        self.isProcessingSubAction(false);
        //        self.isProcessingRequest(false);
        //    });
        //}

        //self.subAction = function () {
        //    return self.isSubscribed() ? self.unsubscribe() : self.subscribe();
        //}

        var search = function (payload) {
            self.isProcessingRequest(true);
            // The datepicker gives us midnight local time, but we're interested in midnight UTC since that's how it's getting stored in the database
            // when you pick a generic (timestamp-less) Measurement date for your report, so reconstruct a UTC date using the same year, month, and date
            //if (payload.Filters.MeasurementDateStart != null) {
            //    var measurementDateStart = moment(payload.Filters.MeasurementDateStart);
            //    payload.Filters.MeasurementDateStart = moment.utc(measurementDateStart.years() + '-' + (measurementDateStart.months() + 1) + '-' + measurementDateStart.date(), 'YYYY-MM-DD');
            //}

            //if (payload.Filters.MeasurementDateEnd != null) {
            //    var measurementDateEnd = moment(payload.Filters.MeasurementDateEnd);
            //    payload.Filters.MeasurementDateEnd = moment.utc(measurementDateEnd.years() + '-' + (measurementDateEnd.months() + 1) + '-' + measurementDateEnd.date(), 'YYYY-MM-DD');
            //}

            if (payload.Filters.ReportSubmitDate != null) {
                var reportSubmitDate = moment(payload.Filters.ReportSubmitDate);
                payload.Filters.ReportSubmitDate = moment.utc(reportSubmitDate.years() + '-' + (reportSubmitDate.months() + 1) + '-' + reportSubmitDate.date(), 'YYYY-MM-DD');
            }

            return webService.Post('/Reports/SiteDashboard/', payload).then(function (data) {
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
        self.modal.comments = ko.observable().extend({ rateLimit: { method: 'notifyWhenChangesStop', timeout: 400 } });
        //self.modal.comments.subscribe(function (c) {
        //    self.modal.disableProceed(!c || c.trim() === '');
        //});
        self.modalReport = ko.observable();
        //self.modalReport.subscribe(function (report) {
        //    self.modal.comments('');
        //    var bodyHtml =
        //        '<div>Please provide comments back to the report author as to why the Unsubmit Request is being dismissed.</div>' +
        //        '<textarea class="form-control" data-bind="value: modal.comments, valueUpdate: \'keyup\'"></textarea>';

        //    self.modal.populate('Dismiss Unsubmit Request (' + report.Site + ' - ' + report.MeasuredOn + ')', bodyHtml, 'Dismiss Request', true, function () {
        //        dismissUnsubmitRequest(report, self.modal.comments());
        //    }, function () {
        //        self.modal.comments('');
        //    }, true);
        //});

        //self.getUnsubmitTitle = function (report) {
        //    if (report.CanUnsubmit) {
        //        if (report.HasRequestedUnsubmit) {
        //            return 'Unsubmit this report per the author\'s request';
        //        }
        //        if (report.Status == 'Draft') {
        //            return 'You can only unsubmit submitted reports';
        //        }
        //        return 'Unsubmit this report';
        //    } else {
        //        if (report.CanRequestUnsubmit && !report.HasRequestedUnsubmit) {
        //            return 'Send request to admin to unsubmit this report';
        //        }
        //        if (report.CanRequestUnsubmit && report.HasRequestedUnsubmit) {
        //            return 'Request to unsubmit this report has already been sent';
        //        }
        //        if (!report.CanRequestUnsubmit) {
        //            return 'You can only unsubmit submitted reports';
        //        }
        //    }
        //};

        //self.requestUnsubmit = function (report) {
        //    if (self.isProcessingRequest()) return;
        //    self.isProcessingRequest(true);
        //    self.pager.waitingForServer(true);
        //    var payload = { ReportId: report.Id };
        //    webService.PostCSRF('/Reports/RequestUnsubmit/', payload, {}, 'html').then(function (data) {
        //        self.pager.query();
        //        $('#bannerContainer').html(data);
        //    }).done();
        //};

        //self.unsubmit = function (report) {
        //    if (self.isProcessingRequest()) return;
        //    self.isProcessingRequest(true);
        //    self.pager.waitingForServer(true);
        //    var payload = { ReportId: report.Id };
        //    webService.PostCSRF('/Reports/Unsubmit/', payload, {}, 'html').then(function (data) {
        //        self.pager.query();
        //        $('#bannerContainer').html(data);
        //    }).done();
        //};

        self.setModalReport = function (report) {
            self.modalReport(report);
        };

        //function dismissUnsubmitRequest(report, comments) {
        //    if (self.isProcessingRequest()) return;
        //    self.isProcessingRequest(true);
        //    self.pager.waitingForServer(true);
        //    var payload = { ReportId: report.Id, Comments: comments };
        //    webService.PostCSRF('/Reports/DismissUnsubmit/', payload, {}, 'html').then(function (data) {
        //        self.pager.query();
        //        $('#bannerContainer').html(data);
        //    }).done();
        //};
    };

    return vm;
})