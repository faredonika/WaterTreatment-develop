'use strict';

define(['jquery', 'knockout', 'App/webService', 'bootstrap', 'bootstrap-multiselect', 'koVal'], function ($, ko, webService) {
    ko.bindingHandlers.notChecked = {
        init: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
            var value = ko.unwrap(valueAccessor());
            $(element).prop('checked', !value);
        },
        update: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
            var value = valueAccessor();
            var valueUnwrapped = ko.unwrap(value);
            ko.bindingHandlers.checkedValue.update(element, function () { return !valueUnwrapped; });
        }
    };

    var viewModel = function (roles, vendors, initialState) {
        var self = this;

        self.msOptions = {
            maxHeight: 400,
            includeSelectAllOption: true,
            enableFiltering: true,
            filterBehavior: 'text',
            enableCaseInsensitiveFiltering: true,
            nonSelectedText: 'Any',
            buttonWidth: '350px'
        };

        self.id = ko.observable(0);
        self.name = ko.observable().extend({ rateLimit: { method: 'notifyWhenChangesStop' } });
        self.firstName = ko.observable();
        self.lastName = ko.observable();
        self.email = ko.observable();
        self.password = ko.observable();
        self.roleId = ko.observable();
        self.vendorId = ko.observable();
        self.selectedState = ko.observable();
        self.isInternational = ko.observable(false);
        self.deactivated = ko.observable(false);
        self.unlocked = ko.observable(false);
        self.siteList = ko.observable();
        self.systemList = ko.observable();

        self.isProcessingRequest = ko.observable(false);

        self.roles = roles;
        self.vendors = vendors;

        self.isActive = true;
        self.isLocked = false;
        self.statusDisplay = 'Active';

        self.TestCheck = function () {
            if (self.selectedState != undefined) {
                if (self.isInternational) {
                    $("#lstSelectState").val($("#lstSelectState").data("default-value"));
                }
            }
            return true;
        };


        $('#lstSelectState').change(function () {
            document.getElementById('chkInternational').checked = false;
        });

        self.UsStates = ko.observableArray(['Alabama', 'Alaska', 'Arizona', 'Arkansas', 'California', 'Colorado', 'Connecticut', 'Delaware', 'Florida',
                                    'Georgia','Hawaii','Idaho','Illinois','Indiana','Iowa','Kansas','Kentucky','Louisiana','Maine','Maryland',
                                    'Massachusetts','Michigan','Minnesota','Mississippi','Missouri','Montana','Nebraska','Nevada','New Hampshire',
                                    'New Jersey','New Mexico','New York','North Carolina','North Dakota','Ohio','Oklahoma','Oregon','Pennsylvania',
                                    'Rhode Island','South Carolina','South Dakota','Tennessee','Texas','Utah','Vermont','Virginia','Washington',
                                    'West Virginia','Wisconsin','Wyoming'])

        if (initialState) {
            self.id(initialState.Id || 0);
            self.name(initialState.Username);
            self.firstName(initialState.FirstName);
            self.lastName(initialState.LastName);
            self.email(initialState.Email);
            self.roleId(initialState.RoleId);
            self.selectedState = (initialState.SelectedState);
            self.isInternational = (initialState.IsInternational);
            self.vendorId(initialState.VendorId);
            self.isActive = initialState.IsActive;
            self.isLocked = initialState.IsLocked;
            self.siteList(initialState.SiteList);
            self.systemList(initialState.SystemList);
            self.isActive = initialState.IsActive;
            self.isLocked = initialState.IsLocked;
            if (!initialState.IsActive) {
                self.statusDisplay = 'Deactivated';
            } else if (initialState.IsLocked) {
                self.statusDisplay = 'Temporarily Locked';
            }
        }

        self.name.extend({ required: true, rateLimit: { method: 'notifyWhenChangesStop', timeout: 400 } }).extend({
            validation: {
                validator: function (val) {
                    self.isProcessingRequest(true);
                    var ret = !/\W/g.test(val);
                    self.isProcessingRequest(false);
                    return ret;
                },
                message: 'Username can only contain alphanumeric characters.'
            }
        }).extend({
            validation: {
                async: true,
                validator: function (name, _, respond) {
                    self.isProcessingRequest(true);
                    webService.Get('/User/ValidateUserName/', { 'name': name, 'id': self.id() }).then(function (data) {
                        respond(data);
                        self.isProcessingRequest(false);
                    });
                },
                message: 'Username must be unique.'
            }
        });

        self.email.extend({ required: true, email: true, rateLimit: { method: 'notifyWhenChangesStop', timeout: 400 } }).extend({
            validation: {
                async: true,
                validator: function (email, _, respond) {
                    self.isProcessingRequest(true);
                    webService.Get('/User/ValidateEmail/', { 'email': email, 'id': self.id() }).then(function (data) {
                        respond(data);
                        self.isProcessingRequest(false);
                    }).done();
                },
                message: 'Email is already associated with an existing account.'
            }
        });

        self.vendorId.extend({
            validation: {
                validator: function (val) {
                    return val !== undefined;
                },
                message: 'User must be assigned to a vendor.'
            }
        });

        var buildList = function(type, list)
        {
            return list
                .filter(function (element) {
                    return element[0] === type;
                })
                .map(function (element) {
                    return element.slice(2);
                })
                .join(',');
        }


        self.selectAll = function () {
            $('#accessTree').jstree("select_all");
        };

        self.unSelectAll = function () {
            $('#accessTree').jstree("deselect_all");
        };

        self.noInvite = ko.observable(false);
        self.passwordEnabled = ko.computed(function () {
            return self.noInvite();
        });

        self.submitLabel = ko.computed(function () {
            return self.noInvite() ? 'Create User' : 'Invite User';
        });

        self.formAction = ko.computed(function () {
            return self.submitLabel().replace(/\W/, '');
        });

        if (initialState.includeInvitation) {
            self.password.extend({
                validation: {
                    validator: function (val) {
                        return (val !== undefined && val !== null && val.trim() !== '') || !self.noInvite();
                    },
                    message: 'A password is required if not sending an invitation.'
                }
            });
        }

        self.canSave = ko.computed(function () {
            if (self.name.isValidating() || self.isProcessingRequest()) {
                return;
            }

            var errors = ko.validation.group([self.name, self.email, self.password, self.vendorId]);

            return errors().length === 0;
        });

        self.save = function () {
            if (self.isProcessingRequest()) return;
            self.isProcessingRequest(true);
            var checked = $('#accessTree').jstree('get_checked');

            // Add sites that are partially checked in the tree because their child systems are checked
            $('.jstree-undetermined').each(function (i, element) {
                checked.push($(element).closest('.jstree-node').attr('id'));
            });

            self.siteList(buildList('S', checked));
            self.systemList(buildList('T', checked));

            if (self.deactivated()) {
                if (window.confirm('Saving these changes will deactivate this user.  Continue with Save?')) {
                    $('#createForm').submit();
                }
            } else {
                $('#createForm').submit();
            }
        };
    };

    return viewModel;
});