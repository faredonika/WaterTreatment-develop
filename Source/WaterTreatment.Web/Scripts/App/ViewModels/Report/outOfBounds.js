define(['knockout', 'jquery', 'pager', 'webService', 'moment', 'bootstrap', 'bootstrap-multiselect'], function (ko, $, pager, webService, moment) {

    var reportListVM = function () {

        var self = this;

        self.reports = ko.observable();

        self.reports.subscribe(function (reports) {
            reports.forEach(function (report) { report.MeasurementDate = moment(report.MeasurementDate).format('L'); });
        });

        self.viewReport = function (report) {
            window.location = '/Reports/ViewSingleReports/' + report.Id;
        };

    };

    return function (systemTypes, sites, initialState) {

        var self = this;

        self.systemTypes = ko.observableArray(systemTypes);
        self.sites = ko.observableArray(sites);
        self.buildings = ko.observableArray();

        self.outOfBoundMeasurements = ko.observableArray();

        self.filters = {
            SystemTypes: ko.observable('Any').extend({ rateLimit: { method: "notifyWhenChangesStop" } }),
            Site: ko.observable('Any').extend({ rateLimit: { method: "notifyWhenChangesStop" } }),
            Buildings: ko.observable('Any').extend({ rateLimit: { method: "notifyWhenChangesStop" } }),
        };

        if (initialState) {
            if (initialState.Filters.Site) {
                self.filters.Site(initialState.Filters.Site);
            }
        }

        var search = function (payload) {
            return webService.Post('/Reports/OutOfBoundsSearch/', payload);
        };

        self.hasBuildings = ko.observable(false);

        self.filters.Site.subscribe(function (siteId) {

            if (siteId === undefined)
                return;

            webService.Get('/Site/Buildings/' + siteId).then(function (data) {
                self.buildings(data);
            });
        });

        self.buildings.subscribe(function (buildings) {
            self.hasBuildings(buildings.length > 0);
        });

        self.pager = new pager(search, self.filters, self.outOfBoundMeasurements);
        self.pager.sortBy('Name');

        self.reportListVM = new reportListVM();

        self.viewReportModel = function (oobm) {
            self.reportListVM.reports(oobm.Reports);
            $('#reportsModel').modal('show');
        };

    };

});