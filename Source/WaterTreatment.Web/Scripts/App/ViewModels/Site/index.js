define(['knockout', 'App/Models/Site/site', 'pager', 'webService', 'bootstrap', 'bootstrap-multiselect', 'App/Bindings/radioButtonGroup'], function (ko, site, pager, webService) {

    var indexViewModel = function (sytemTypes) {

        var self = this;

        self.sites = ko.observableArray();

        self.systemTypes = sytemTypes;
        self.isProcessingRequest = ko.observable(false);

        self.filters = {
            Name: ko.observable($('#Name')[0].value).extend({ rateLimit: { method: "notifyWhenChangesStop" } }),
            Location: ko.observable($('#Location')[0].value).extend({ rateLimit: { method: "notifyWhenChangesStop" } }),
            BuildingName: ko.observable($('#BuildingName')[0].value).extend({ rateLimit: { method: "notifyWhenChangesStop" } }),
            SystemTypes: ko.observable().extend({ rateLimit: { method: "notifyWhenChangesStop" } }),
            IsActive: ko.observable('true').extend({ rateLimit: { method: "notifyWhenChangesStop" } }),
        };

        var search = function (payload) {
            self.isProcessingRequest(true);
            return webService.Post('/Site/Search/', payload).then(function (data) {
                self.isProcessingRequest(false);
                return data;
            });;
        };

        self.pager = new pager(search, self.filters, self.sites, site);
        self.pager.sortBy('Name');

    };

    return indexViewModel;
})