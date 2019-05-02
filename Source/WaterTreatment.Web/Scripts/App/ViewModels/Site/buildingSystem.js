'use strict';

define(['knockout'], function (ko) {

    var system = function (systemTypes, data) {

        var self = this;

        self.initialState = {
            id: 0,
            location: '',
            description: '',
            systemId: systemTypes[0].Id
        };

        self.savedData = {
            id: 0,
            location: '',
            description: '',
            systemId: ''
        };

        self.saveLabel = ko.observable('Add');

        if (data !== undefined) {
            self.initialState.id = data.Id;
            self.initialState.location = data.Location;
            self.initialState.description = data.Description;
            self.initialState.systemId = data.SystemTypeId;

            self.savedData.id = self.initialState.id;
            self.savedData.location = self.initialState.location;
            self.savedData.description = self.initialState.description;
            self.savedData.systemId = self.initialState.systemId;

            self.saveLabel('Update');
        }

        self.systemType = ko.observable(self.initialState.systemId);
        self.location = ko.observable(self.initialState.location).extend({ required: true });
        self.description = ko.observable(self.initialState.description).extend({ required: true });

        self.displayName = ko.computed(function () {
            var sId = self.systemType();
            return systemTypes.filter(function (s) { return s.Id == sId }).pop().Name;
        }, self);

        self.inEditMode = ko.observable(data === undefined);

        self.hasData = function () {
            return self.savedData.location !== '';
        };

        self.edit = function () {
            self.inEditMode(true);
        };

        self.cancel = function () {
            self.location(self.savedData.location);
            self.systemType(self.savedData.systemId);
            self.description(self.savedData.description);
            self.inEditMode(false);
        }

        self.isDirty = ko.computed(function () {

            var location = self.location();
            var systemType = self.systemType();
            var description = self.description();

            return location !== self.initialState.location || systemType !== self.initialState.systemId || description !== self.initialState.description;
        }, self);

        self.bake = function () {
            return {
                Id: self.initialState.id,
                Location: self.savedData.location,
                Description: self.savedData.description,
                SystemTypeId: self.savedData.systemId
            };
        }

        self.isValid = ko.computed(function () {
            var errors = ko.validation.group([self.location, self.description]);

            return errors().length === 0;
        });

        self.save = function () {
            var errors = ko.validation.group([self.location, self.description]);

            if (errors().length > 0) {
                errors.showAllMessages();
                return;
            }

            self.savedData.location = self.location();
            self.savedData.description = self.description();
            self.savedData.systemId = self.systemType();
            self.inEditMode(false);
            self.saveLabel('Update');
        }

    }

    return system;

});