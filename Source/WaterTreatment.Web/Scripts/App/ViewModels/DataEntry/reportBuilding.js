define(['knockout', 'App/ViewModels/DataEntry/reportSystem', 'App/webservice'], function (ko, systemVM, webService) {

    return function (data, isProcessingRequestObs) {

        var self = this;

        self.id = ko.observable(data.Id);
        self.name = ko.observable(data.Name);
        self.systems = ko.observableArray(data.Systems.map(function (s) { return new systemVM(s, isProcessingRequestObs); }));

        self.canSubmit = ko.computed(function () {
            return self.systems().every(function (s) { return s.canSave(); });
        }, self);

        self.isDirty = ko.computed(function () {
            return self.systems().some(function (s) { return s.isDirty(); });
        }, self);

        self.bake = function () {
            return {
                Id: self.id(),
                Name: self.name(),
                Systems: self.systems().map(function (s) { return s.bake(); }),
            };
        };

    };

});