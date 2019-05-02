define(['knockout', 'bootstrap', 'bootstrap-multiselect'], function (ko) {

    var createVM = function (sites) {

        var self = this;

        self.sites = sites;

        self.siteId = ko.observable();
        self.use = ko.observable('Measurement');

        self.isProcessingRequest = ko.observable(false);
        self.create = function () {
            if (self.isProcessingRequest()) return;
            self.isProcessingRequest(true);
            $('#createForm').submit();
        };

    };

    return createVM;

});