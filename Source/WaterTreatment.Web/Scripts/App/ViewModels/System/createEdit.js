'use strict';
define(['knockout', 'App/ViewModels/System/Parameter', 'webService', 'App/compareEntities', 'koVal', 'bootstrap', 'bootstrap-multiselect', 'App/Bindings/radioButtonGroup'], function (ko, Parameter, webService, compareEntities) {
    var CEViewModel = function (parameterTypes, sites, data) {

        var self = this;

        self.sites = sites;

        self.initialState = {
            id: 0,
            name: '',
            isActive: 'true',
            parameters: []
        };

        self.initialParameters = [];

        if (typeof data !== 'undefined') {
            self.initialState.id = data.Id;
            self.initialState.name = data.Name;
            self.initialState.isActive = data.IsActive ? 'true' : 'false';
            self.initialState.parameters = data.Parameters.map(function (p) { return new Parameter(self, parameterTypes, p); });

            self.initialParameters = data.Parameters.map(function (p) { return new Parameter(self, parameterTypes, p); });
        }

        self.name = ko.observable(self.initialState.name).extend({ required: true });
        self.isActive = ko.observable(self.initialState.isActive).extend({ required: true });
        self.parameters = ko.observableArray(self.initialParameters);

        self.isSaving = ko.observable(false);
        self.isProcessingRequest = ko.observable(false);

        self.areParametersDirty = ko.computed(function () {
            return self.parameters().some(function (p) { return p.isDirty(); });
        }, self);

        self.isDirty = ko.computed(function () {


            var parametersChanged = !compareEntities(self.initialState.parameters, self.parameters());

            var isSaving = self.isSaving();
            var name = self.name();
            var isActive = self.isActive();
            var areParametersDirty = self.areParametersDirty();

            return !isSaving && (name !== self.initialState.name || isActive !== self.initialState.isActive || areParametersDirty || parametersChanged);
        }, self);

        self.addParameter = function () {
            var p = new Parameter(self, parameterTypes);
            self.parameters.push(p);
        };

        self.anyEditing = ko.computed(function () {
            return self.parameters().some(function (p) { return p.anyEditing(); });
        }, self);

        self.bake = function () {
            return {
                Id: self.initialState.id,
                Name: self.name(),
                IsActive: self.isActive(),
                Parameters: self.parameters().map(function (p) { return p.bake(); })
            }
        };

        self.isValid = ko.computed(function () {
            var errors = ko.validation.group([self.name]),
                allParamsValid = self.parameters().reduce(function (val, param) { return val && param.isValid(); }, true);

            return errors().length === 0 && allParamsValid;
        });

        self.save = function () {
            if (self.isProcessingRequest()) return;
            self.isProcessingRequest(true);
            var errors = ko.validation.group([self.name]);

            if (errors().length > 0) {
                errors.showAllMessages();
                self.isProcessingRequest(false);
                return;
            }

            var confirmMessage = ' You have parameters or bounds that have not been saved. Saving the system type will not save those parameters. Would you like to continue saving the system type?';

            if (self.anyEditing() && !confirm(confirmMessage)) {
                self.isProcessingRequest(false);
                return;
            }

            self.parameters().forEach(function (p) {
                p.cancel();
                p.bounds().forEach(function (b) {
                    b.cancel();
                });
            });

            var url = '';

            if (self.initialState.id === 0) {
                url = '/System/Create/';
            }
            else {
                url = '/System/Edit/' + self.initialState.id;
            }
            self.isSaving(true);
            webService.SubmitFormCSRF(url, self.bake());

        };

    };

    return CEViewModel;

})