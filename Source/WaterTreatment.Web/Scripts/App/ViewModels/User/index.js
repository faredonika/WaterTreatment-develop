define(['knockout', 'pager', 'webService'], function (ko, pager, webService) {
    var vm = function (sites, vendors, roles, managedSites, initialState) {
        var self = this;

        sites.splice(0, 0, { Id: 0, Name: 'Unassigned' });

        self.users = ko.observableArray();
        self.roles = ko.observable(roles);
        self.sites = ko.observable(sites);
        self.managedSites = ko.observable(managedSites);
        self.vendors = ko.observable(vendors);
        self.statuses = ko.observable([
            { Name: 'Active', Value: 'active' },
            { Name: 'Deactivated', Value: 'inactive' }
        ]);
        self.siteAccess = ko.observable();
        self.isProcessingRequest = ko.observable(false);

        self.msOptions = {
            maxHeight: 400,
            includeSelectAllOption: true,
            enableFiltering: true,
            filterBehavior: 'text',
            enableCaseInsensitiveFiltering: true,
            nonSelectedText: 'Any',
            buttonWidth: '350px'
        };

        self.filters = {
            Name: ko.observable().extend({ rateLimit: { method: 'notifyWhenChangesStop' } }),
            Email: ko.observable().extend({ rateLimit: { method: 'notifyWhenChangesStop' } }),
            Roles: ko.observable().extend({ rateLimit: { method: 'notifyWhenChangesStop' } }),
            Sites: ko.observable().extend({ rateLimit: { method: 'notifyWhenChangesStop' } }),
            Vendors: ko.observable().extend({ rateLimit: { method: 'notifyWhenChangesStop' } }),
            Statuses: ko.observable().extend({ rateLimit: { method: 'notifyWhenChangesStop' } })
        };

        if (initialState) {
            self.filters.Sites(initialState.SiteFilter);
        }
        self.filters.Statuses(['active']);

        var search = function (payload) {
            self.isProcessingRequest(true);
            return webService.Post('/User/SearchUsers/', payload).then(function (data) {
                self.isProcessingRequest(false);
                return data;
            });
        };

        var processingFeedback = function () {
            if (self.isProcessingRequest()) return true;
            self.isProcessingRequest(true);
            self.pager.waitingForServer(true);
            return false;
        };

        self.pager = new pager(search, self.filters, self.users);
        self.pager.sortDirection(false);
        self.pager.sortBy('Name');

        self.unlock = function (user) {
            if (processingFeedback()) return;
            var payload = { UserId: user.Id };

            webService.PostCSRF('/User/UnlockUser/', payload, {}, 'html').then(function (data) {
                self.pager.query();
                $('#bannerContainer').html(data);
            }).done();
        };

        self.deactivate = function (user) {
            if (self.isProcessingRequest()) return;
            self.isProcessingRequest(true);
            if (window.confirm('Are you sure you want to deactivate user ' + user.Name + ' (' + user.Role + ')?')) {
                self.pager.waitingForServer(true);
                var payload = { UserId: user.Id };

                webService.PostCSRF('/User/DeactivateUser/', payload, {}, 'html').then(function (data) {
                    self.pager.query();
                    $('#bannerContainer').html(data);
                }).done();
            } else {
                self.isProcessingRequest(false);
            }
        };

        self.grantSiteAccess = function (user) {
            if (processingFeedback()) return;
            webService.PostCSRF('/User/GrantSiteAccess/', { siteId: self.siteAccess().Id, userId: user.Id }, {}, 'html').then(function (data) {
                self.pager.query();
                $('#bannerContainer').html(data);
            });
        };

        self.revokeSiteAccess = function (user) {
            if (processingFeedback()) return;
            webService.PostCSRF('/User/RevokeSiteAccess/', { siteId: self.siteAccess().Id, userId: user.Id }, {}, 'html').then(function (data) {
                self.pager.query();
                $('#bannerContainer').html(data);
            });
        };

        self.accessLabel = function (prefix) {
            if (self.managedSites().length === 0)
                return '';

            return ko.computed(function () {
                return prefix + ' ' + self.siteAccess().Name;
            });
        }

        self.canEdit = function (user) {
            var ids = user.SiteIdList.split(','),
                managedIds = managedSites.map(function (site) {
                return site.Id.toString();
                });

            if (initialState.Role === 'System Admin')
                return true;

            return ko.computed(function () {
                return ids.some(function (id) {
                    return managedIds.indexOf(id) !== -1;
                });
            });
        }

        self.hasSiteAccess = function (user) {
            var ids = user.SiteIdList.split(',');

            if (self.managedSites().length === 0)
                return false;

            return ko.computed(function () {
                return ids.indexOf(self.siteAccess().Id.toString()) !== -1;
            });
        };

        self.noSiteAccess = function (user) {
            var ids = user.SiteIdList.split(',');

            if (self.managedSites().length === 0)
                return false;

            return ko.computed(function () {
                return ids.indexOf(self.siteAccess().Id.toString()) === -1;
            });
        }
    };

    return vm;
})