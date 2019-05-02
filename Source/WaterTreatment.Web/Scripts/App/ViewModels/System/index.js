define(['knockout', 'App/ViewModels/System/system', 'pager', 'webService', 'App/Bindings/radioButtonGroup'], function (ko, system, pager, webService) {

    var indexViewModel = function () {

        var self = this;

        self.systems = ko.observableArray();
        self.isProcessingRequest = ko.observable(false);

        self.filters = {
            Name: ko.observable($('#Name')[0].value).extend({ rateLimit: { method: "notifyWhenChangesStop" } }),
            HasParameters: ko.observable('Any').extend({ rateLimit: { method: "notifyWhenChangesStop" } }),
            InUse: ko.observable('Any').extend({ rateLimit: { method: "notifyWhenChangesStop" } }),
            IsActive: ko.observable('true').extend({ rateLimit: { method: "notifyWhenChangesStop" } }),
        };

        var search = function (payload) {
            return webService.Post('/System/Search/', payload);
        };

        self.pager = new pager(search, self.filters, self.systems, system);
        self.pager.sortBy('Name');

    };

    return indexViewModel;
})