'use strict';

define(['jquery', 'knockout', 'App/states', 'App/webService', 'koVal'], function ($, ko, states, webService) {
    var viewModel = function (initialState) {
        var self = this;

        self.name = ko.observable().extend({ rateLimit: { method: 'notifyWhenChangesStop' } }).extend({ required: true });
        self.phone = ko.observable().extend({ required: true });
        self.contact = ko.observable().extend({ required: true });

        self.id = ko.observable(0);
        self.address = ko.observable().extend({ required: true });
        self.city = ko.observable().extend({ required: true });
        self.state = ko.observable().extend({ required: true });
        self.zipCode = ko.observable().extend({ required: true });

        self.states = ko.observable(states);

        self.isProcessingRequest = ko.observable(false);

        if (initialState) {
            self.name(initialState.Name);
            self.phone(initialState.Phone);
            self.contact(initialState.PointOfContact);

            self.id(initialState.Id || 0);
            self.address(initialState.Address);
            self.city(initialState.City);
            self.state(initialState.State);
            self.zipCode(initialState.ZipCode);
        }

        self.name.extend({ rateLimit: { method: 'notifyWhenChangesStop', timeout: 400 } }).extend({
            validation: {
                async: true,
                validator: function (name, _, respond) {
                    self.isProcessingRequest(true);
                    webService.Get('/User/ValidateVendorName/', { 'name': name, 'id': self.id() }).then(function (data) {
                        respond(data);
                        self.isProcessingRequest(false);
                    });
                },
                message: 'Name must be unique.'
            }
        });

        self.canSave = ko.computed(function () {
            if (self.isProcessingRequest()) return false;

            var errors = ko.validation.group([self.name, self.phone, self.contact, self.address, self.city, self.state, self.zipCode]);
            return errors().length === 0;
        });

        self.save = function () {
            if (self.isProcessingRequest()) return;
            self.isProcessingRequest(true);
            $('#vendorForm').submit();
        };
    };

    return viewModel;
});