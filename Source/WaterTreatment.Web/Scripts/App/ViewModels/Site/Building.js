'use strict';

define(['knockout', 'App/ViewModels/Site/buildingSystem', 'util', 'koVal', 'App/Bindings/radioButtonGroup'], function (ko, System, util) {

    var Building = function (site, systemTypes, data) {

        var self = this;

        self.initialState = {
            id: 0,
            name: '',
            number: '',
            RPUID: '',
            RPSUID: '',
            isActive: 'true',
            systems: []
        };

        self.savedData = {
            id: 0,
            name: '',
            number: '',
            RPUID: '',
            RPSUID: '',
            isActive: 'true',
            systems: []
        };

        self.saveLabel = ko.observable('Add');

        if (data !== undefined) {
            self.initialState.id = data.Id;
            self.initialState.name = data.Name;
            self.initialState.number = data.BuildingNumber;
            self.initialState.RPUID = data.RPUID;
            self.initialState.RPSUID = data.RPSUID;
            self.initialState.isActive = data.IsActive ? 'true' : 'false';
            self.initialState.systems = data.Systems.map(function (s) { return new System(systemTypes, s); });

            self.savedData.id = data.Id;
            self.savedData.name = data.Name;
            self.savedData.number = data.BuildingNumber;
            self.savedData.RPUID = data.RPUID;
            self.savedData.RPSUID = data.RPSUID;
            self.savedData.isActive = data.IsActive ? 'true' : 'false';
            self.savedData.systems = self.initialState.systems;

            self.saveLabel('Update');
        }

        self.name = ko.observable(self.initialState.name).extend({ required: true }).extend({
            validation: {
                validator: function (val) {
                    return site.buildingValidator(val);
                },
                message: 'The building name must be unique within a site.'
            }
        });

        self.number = ko.observable(self.initialState.number);
        self.RPUID = ko.observable(self.initialState.RPUID);
        self.RPSUID = ko.observable(self.initialState.RPSUID);
        self.isActive = ko.observable(self.initialState.isActive).extend({ required: true });

        self.systems = ko.observableArray(self.initialState.systems);

        self.hasData = function () {
            return self.savedData.name !== '';
        };

        self.inEditMode = ko.observable(data === undefined);

        self.edit = function () {
            self.inEditMode(true);
        };

        self.cancel = function () {
            self.name(self.savedData.name);
            self.number(self.savedData.number);
            self.RPUID(self.savedData.RPUID);
            self.RPSUID(self.savedData.RPSUID);
            self.systems(self.savedData.systems);
            self.isActive(self.savedData.isActive);
            self.inEditMode(false);
        };

        self.areSystemsDirty = ko.computed(function () {
            return self.systems().some(function (s) { return s.isDirty(); });
        }, self);

        self.isDirty = ko.computed(function () {

            var name = self.name();
            var number = self.number();
            var RPUID = self.RPUID();
            var RPSUID = self.RPSUID();
            var isActive = self.isActive();
            var systemDirty = self.areSystemsDirty();


            return name !== self.initialState.name
                || number !== self.initialState.number
                || RPUID !== self.initialState.RPUID
                || RPSUID !== self.initialState.RPSUID
                || isActive !== self.initialState.isActive
                || systemDirty;
        }, self);

        self.addSystem = function () {
            var s = new System(systemTypes);
            self.systems.push(s);
        };

        self.removeSystem = function (system) {
            var message = 'To delete this system, please type the word \"delete\" and click OK. Please note that these changes do not take effect until the site is saved.';
            util.promptForCommand(message, 'delete', function () {
                self.systems.remove(system);
            }, function () {
            });
        };

        self.cancelSystem = function (system) {
            if (system.hasData()) {
                system.cancel();
            }
            else {
                self.systems.remove(system);
            }
        };

        self.anyEditing = ko.computed(function () {
            var systemsEditing = self.systems().some(function (s) { return s.inEditMode(); });
            return self.inEditMode() || systemsEditing;
        }, self);

        self.bake = function () {
            return {
                Id: self.savedData.id,
                Name: self.savedData.name,
                BuildingNumber: self.savedData.number,
                RPUID: self.savedData.RPUID,
                RPSUID: self.savedData.RPSUID,
                IsActive: self.savedData.isActive,
                Systems: self.savedData.systems.map(function (s) { return s.bake(); })
            };
        };

        self.isValid = ko.computed(function () {
            var errors = ko.validation.group([self.name]);

            return errors().length === 0 && self.systems().reduce(function (val, system) { return val && system.isValid(); }, true);
        });

        self.save = function () {
            var errors = ko.validation.group([self.name]);

            if (errors().length > 0) {
                errors.showAllMessages();
                return;
            }

            self.savedData.name = self.name();
            self.savedData.number = self.number();
            self.savedData.RPUID = self.RPUID();
            self.savedData.RPSUID = self.RPSUID();
            self.savedData.systems = self.systems();
            self.savedData.isActive = self.isActive();
            ko.validation.validateObservable(site.buildings);
            self.inEditMode(false);
            self.saveLabel('Update');
        };

    };

    return Building;

});