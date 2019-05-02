'use strict';

define(['jquery', 'knockout', 'App/ViewModels/Site/building', 'webService', 'util', 'koVal', 'bootstrap', 'bootstrap-multiselect', 'App/Bindings/radioButtonGroup'], function ($, ko, Building, webService, util) {
    
    var site = function (systemTypes, data) {

        var self = this;

        self.activeSystemTypes = systemTypes.filter(function (systemTypes) { return systemTypes.IsActive == true; });

        self.systemTypes = systemTypes;

        self.initialState = {
            id: 0,
            name: '',
            location: '',
            isActive: 'true',
            buildings: []
        };

        self.buildingValidator = function (name) {

            if (self.buildings === undefined)
                return true;

            return self.buildings().filter(function (d) { return d.name().trim().toLowerCase() === name.trim().toLowerCase() }).length == 1;
        };

        if (typeof data !== 'undefined') {
            self.initialState.id = data.Id;
            self.initialState.name = data.Name;
            self.initialState.location = data.Location;
            self.initialState.isActive = data.IsActive ? 'true' : 'false';
            self.initialState.buildings = data.Buildings.map(function (b) { return new Building(self, systemTypes, b); });
        }

        self.name = ko.observable(self.initialState.name).extend({ required: true });
        self.location = ko.observable(self.initialState.location).extend({ required: true });;
        self.isActive = ko.observable(self.initialState.isActive).extend({ required: true });

        self.buildings = ko.observableArray(self.initialState.buildings).extend({
            validation: {
                validator: function (val) {
                    return val.some(function (b) { return b.hasData(); });
                },
                message: 'At least one saved building must be added.'
            }
        });

        self.isSaving = ko.observable(false);
        self.isProcessingRequest = ko.observable(false);

        self.areBuildingsDirty = ko.computed(function () {
            return self.buildings().some(function (b) { return b.isDirty(); });
        }, self);

        self.isDirty = ko.computed(function () {

            var isSaving = self.isSaving();
            var name = self.name();
            var location = self.location();
            var isActive = self.isActive();
            var areBuildingsDirty = self.areBuildingsDirty();

            return !isSaving && (name !== self.initialState.name || location !== self.initialState.location || isActive !== self.initialState.isActive || areBuildingsDirty);
        }, self);

        self.addBuilding = function () {
            var b = new Building(self, systemTypes);
            self.buildings.push(b);
        };

        self.removeBuilding = function (building) {
            var message = 'To delete this building, please type the word \"delete\" and click OK. Please note that these changes do not take effect until the site is saved.';
            util.promptForCommand(message, 'delete', function () {
                self.buildings.remove(building);
            }, function () {
            });
        };

        self.cancelBuilding = function (building) {
            if (building.hasData()) {
                building.cancel();
            }
            else {
                self.buildings.remove(building);
            }
        };

        self.anyEditing = ko.computed(function () {
            return self.buildings().some(function (b) { return b.anyEditing(); });
        }, self);

        self.bake = function () {
            return {
                Id: self.initialState.id,
                Name: self.name(),
                Location: self.location(),
                IsActive: self.isActive(),
                Buildings: self.buildings().map(function (b) { return b.bake(); })
            };
        };

        self.isValid = ko.computed(function () {
            var errors = ko.validation.group([self.name, self.location, self.buildings]);

            return errors().length === 0 && self.buildings().length !== 0 && self.buildings().reduce(function (val, building) { return val && building.isValid(); }, true);
        });

        self.save = function () {
            if (self.isProcessingRequest()) return;
            self.isProcessingRequest(true);
            var errors = ko.validation.group([self.name, self.location, self.buildings]);

            if (errors().length > 0) {
                errors.showAllMessages();
                self.isProcessingRequest(false);
                return;
            }
            
            var confirmMessage = 'You have buildings or systems which have not been added to the site.  Saving the site will not save those buildings or systems. Would you like to proceed with saving the site?';

            if (self.anyEditing() && !confirm(confirmMessage)) {
                self.isProcessingRequest(false);
                return;
            }

            self.buildings().forEach(function (b) {
                self.cancelBuilding(b);
                b.systems().forEach(function (s) {
                    b.cancelSystem(s);
                });
            });

            var url = '';

            if (self.initialState.id === 0) {
                url = '/Site/Create/';
            }
            else {
                url = '/Site/Edit/' + self.initialState.id;
            }
            self.isSaving(true);
            webService.SubmitFormCSRF(url, self.bake());
        };

    };

    return site;

});