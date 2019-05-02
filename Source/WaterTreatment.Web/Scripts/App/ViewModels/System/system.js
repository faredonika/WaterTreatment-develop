define(['knockout'], function (ko) {
    var system = function (data) {

        var self = this;

        self.id = ko.observable(data.Id || '');
        self.name = ko.observable(data.Name || '');
        self.hasParameters = ko.observable(data.HasParameters || '');
        self.inUse = ko.observable(data.InUse || '');

        self.edit = function (isProcessingRequestObs) {
            if (isProcessingRequestObs()) return;
            isProcessingRequestObs(true);
            window.location = self.getEditUrl();
        }

        self.getEditUrl = ko.computed(function () {
            return "/System/Edit/" + self.id();
        }, self);

    };

    return system;
});