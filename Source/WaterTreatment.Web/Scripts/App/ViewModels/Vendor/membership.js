'use strict';

define(['jquery', 'knockout', 'App/webService', 'App/pager'], function ($, ko, webService, pager) {
    var viewModel = function (initialState) {
        var self = this;

        self.id = ko.observable(initialState.Id);

        self.members = ko.observable(initialState.Members);
        self.removals = ko.observableArray();

        self.hasRemovals = ko.computed(function () {
            return self.removals().length > 0;
        });

        self.nonMembers = ko.observableArray();
        self.additions = ko.observableArray();

        self.isProcessingRequest = ko.observable(false);

        self.filters = {
            Name: ko.observable().extend({ rateLimit: { method: 'notifyWhenChangesStop' } }),
            Email: ko.observable().extend({ rateLimit: { method: 'notifyWhenChangesStop' } }),
            Roles: ko.observable().extend({ rateLimit: { method: 'notifyWhenChangesStop' } }),
            Sites: ko.observable().extend({ rateLimit: { method: 'notifyWhenChangesStop' } }),
            Vendors: ko.observable().extend({ rateLimit: { method: 'notifyWhenChangesStop' } }),
            Statuses: ko.observable().extend({ rateLimit: { method: 'notifyWhenChangesStop' } })
        };

        var search = function (payload) {
            self.isProcessingRequest(true);
            return webService.Post('/User/SearchVendorMembership/' + self.id(), payload).then(function(data) {
                self.isProcessingRequest(false);
                return data;
            });
        };

        self.pager = new pager(search, self.filters, self.nonMembers);
        self.pager.sortDirection(false);
        self.pager.sortBy('Name');

        self.hasAdditions = ko.computed(function () {
            return self.additions().length > 0;
        });

        self.add = function () {
            if (self.isProcessingRequest()) return;
            self.isProcessingRequest(true);
            webService.PostCSRF('/User/AddVendorMember', { id: self.id(), additions: self.additions().join(',') }, {}).then(function (members) {
                self.members(members);
                self.pager.query();
                self.additions([]);
            });
        }

        self.remove = function () {
            if (self.isProcessingRequest()) return;
            self.isProcessingRequest(true);
            webService.PostCSRF('/User/RemoveVendorMember', { id: self.id(), removals: self.removals().join(',') }, {}).then(function (members) {
                self.members(members);
                self.pager.query();
                self.removals([]);
            });
        }
    };

    return viewModel;
});