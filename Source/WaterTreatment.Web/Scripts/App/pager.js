define(['knockout'], function (ko) {
    return function (search, filters, resultBinding, model, preBindTransform) {
        var self = this;

        var defaultPagerSize = 10;

        self.maxResults = ko.observable(defaultPagerSize).extend({ rateLimit: { method: "notifyWhenChangesStop" } });
        self.search = search;
        self.filters = filters;
        self.resultBinding = resultBinding || ko.observableArray([]);
        self.modelMap = model || function (d) { return d; }
        self.preBindTransform = preBindTransform || function(results) {
            self.totalResults(results.Total);
            return results.Results.map(function (d) { return new self.modelMap(d); });
        };

        self.offset = ko.observable();
        self.actualIndex = ko.observable();
        self.foundEnd = ko.observable(false);
        self.waitingForServer = ko.observable(true);

        self.sortDirection = ko.observable(true);
        self.sortBy = ko.observable();

        self.totalResults = ko.observable(0);

        self.currentIndicies = ko.computed(function () {
            var firstIndex = self.actualIndex() + 1;
            var lastIndex = firstIndex + self.resultBinding().length - 1;
            return firstIndex + "-" + lastIndex;
        });

        self.sort = function (SortBy) {
            if (self.sortBy() == SortBy) {
                self.sortDirection(!self.sortDirection());
            } else {
                self.sortBy(SortBy);
                self.sortDirection(true);
            }
            self.offset(0);
        };

        self.sortArrow = function (sortBy) {
            return ko.computed(function () {
                var icon = 'fa fa-fw';
                if (self.sortBy() == sortBy) {
                    icon += ' fa-long-arrow-' + (self.sortDirection() ? 'up' : 'down');
                }
                return icon;
            }, self);
        };
        
        self.mapFilters = ko.computed(function () {
            var map = {};
            for (var key in self.filters) {
                var value = ko.utils.unwrapObservable(self.filters[key]) || null;
                if (Array.isArray(value)) {
                    value = value.join(",");
                    if (value === "") {
                        value = null;
                    }
                }
                map[key] = value;
            }
            return map;
        });

        self.criteria = ko.computed(function () {
            return {
                MaxResults: Number(self.maxResults()) + 1,
                Offset: self.offset(),
                ShouldForwardSearch: self.sortDirection(),
                SortBy: self.sortBy(),
                Filters: self.mapFilters()
            };
        }).extend({ throttle: 500 });

        self.mapFilters.subscribe(function () {
            self.reset();
        });

        self.latestQuery = 0;
        self.lastQuery = 0;
        self.query = function () {
            self.waitingForServer(true);

            var set = Number(self.maxResults());

            var criteria = self.criteria();
            var currentQuery = ++self.latestQuery;
            self.search(criteria).then(function (d) {
                try {
                    if (currentQuery >= self.lastQuery) {
                        self.actualIndex(self.offset());
                        self.waitingForServer(false);
                        self.foundEnd(Number(self.offset()) + Number(self.maxResults()) >= d.Total);

                        var prebinded = self.preBindTransform(d);

                        self.resultBinding(prebinded.slice(0, set));
                        self.lastQuery = currentQuery;
                    }
                } catch (e) {
                    console.log(e);
                }
            });
        };

        self.criteria.subscribe(self.query);

        self.reset = function () {
            self.offset(null);
            self.offset(0);
        };

        self.IsPrevEnabled = ko.computed(function () {
            return !self.waitingForServer() && self.offset() > 0;
        });

        self.IsNextEnabled = ko.computed(function () {
            return !self.waitingForServer() && !self.foundEnd();
        });

        self.clearFilters = function () {
            for (var key in self.filters) {
                if (ko.isObservable(self.filters[key])) {
                    self.filters[key](null);
                }
            }
        };

        self.next = function () {
            self.offset(self.offset() + Number(self.maxResults()));
        };

        self.prev = function () {
            self.offset(self.offset() - Number(self.maxResults()));
            if (self.offset() < 0) {
                self.offset(0);
            }
        };

        self.reset();
    };
});