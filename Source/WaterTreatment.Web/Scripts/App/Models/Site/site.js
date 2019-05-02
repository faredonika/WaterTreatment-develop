define(['knockout'], function (ko) {
    var system = function (data) {

        var self = this;

        self.id = ko.observable(data.Id);
        self.name = ko.observable(data.Name);
        self.location = ko.observable(data.Location);
        self.buildingCount = ko.observable(data.BuildingCount);

        self.edit = function (isProcessingRequestObs) {
            if (isProcessingRequestObs()) return;
            isProcessingRequestObs(true);
            window.location = self.getEditUrl();
        }

        self.getEditUrl = ko.computed(function () {
            return "/Site/Edit/" + self.id();
        }, self);

    };

    return system;
});