define(['knockout', 'util', 'koVal'], function (ko, util) {

    var bounds = function (parent, data) {

        var self = this;

        self.initialState = {
            id: 0,
            type: 'Value',
            range: 'Both',
            minValue: '',
            maxValue: '',
            minDescription: '',
            maxDescription: '',
            siteId: 0,
            isEnforced: 'true'
        };

        self.savedData = {
            id: 0,
            type: 'Value',
            range: 'Both',
            minValue: '',
            maxValue: '',
            siteId: 0,
            minDescription: '',
            maxDescription: '',
            isEnforced: 'true'
        };

        self.saveLabel = ko.observable('Add');

        if (typeof data !== 'undefined') {
            self.initialState.id = data.Id;
            self.initialState.type = data.Type;
            self.initialState.range = data.Range;
            self.initialState.minValue = data.MinValue;
            self.initialState.maxValue = data.MaxValue;
            self.initialState.minDescription = data.MinDescription;
            self.initialState.maxDescription = data.MaxDescription;
            self.initialState.siteId = data.SiteId;
            self.initialState.isEnforced = data.IsEnforced ? 'true' : 'false';

            self.savedData.id = self.initialState.id;
            self.savedData.type = self.initialState.type;
            self.savedData.range = self.initialState.range;
            self.savedData.minValue = self.initialState.minValue;
            self.savedData.maxValue = self.initialState.maxValue;
            self.savedData.minDescription = self.initialState.minDescription;
            self.savedData.maxDescription = self.initialState.maxDescription;
            self.savedData.siteId = self.initialState.siteId;
            self.savedData.isEnforced = self.initialState.isEnforced;

            self.saveLabel('Update');
        }

        self.type = ko.observable(self.initialState.type).extend({ required: true });
        self.range = ko.observable(self.initialState.range).extend({ required: true });
        
        self.type.subscribe(function (t) {
            if (t === 'Value') {
                self.range('Both');
            }
            else {
                self.minValue(1);
                self.maxValue(1);
            }
        });

        self.valueType = ko.computed(function () {
            return self.type() === 'Value';
        }, self);

        self.minValue = ko.observable(self.initialState.minValue).extend({
            validation: {
                validator: function (val) {
                    if (self.range() === 'Minimum' || self.range() === 'Both') {
                        return val !== '';
                    }
                    return true;
                },
                message: 'Minimum value is required.'
            }
        }).extend({
            number: {
                onlyIf: function () {
                    return self.range() === 'Minimum' || self.range() === 'Both';
                }
            }
        });

        self.minDescription = ko.observable(self.initialState.minDescription);
        self.maxDescription = ko.observable(self.initialState.maxDescription);

        self.maxValue = ko.observable(self.initialState.maxValue).extend({
            validation: {
                validator: function (val) {
                    if (self.range() === 'Maximum' || self.range() === 'Both') {
                        return val !== '';
                    }
                    return true;
                },
                message: 'Maximum value is required.'
            }
        }).extend({
            number: {
                onlyIf: function () {
                    return self.range() === 'Maximum' || self.range() === 'Both';
                }
            }
        });

        self.minValueVisible = ko.computed(function () {
            return self.range() === 'Minimum' || self.range() === 'Both';
        }, self);

        self.maxValueVisible = ko.computed(function () {
            return self.range() === 'Maximum' || self.range() === 'Both';
        }, self);

        self.range.subscribe(function () {
            ko.validation.validateObservable(self.minValue);
            ko.validation.validateObservable(self.maxValue);
        });

        self.siteId = ko.observable(self.initialState.siteId);
        self.isGlobal = ko.observable(self.initialState.siteId === 0 ? 'true' : 'false');
        
        self.showSite = ko.computed(function () {
            return self.isGlobal() !== 'true'
        }, self);

        self.isEnforced = ko.observable(self.initialState.isEnforced).extend({ required: true });

        self.hasData = function () {
            return self.savedData.minValue !== '' || self.savedData.maxValue !== '';
        };

        self.displayName = ko.computed(function () {

            var display = self.range() + ' ',
                min = self.minValue(),
                max = self.maxValue();

            if (!self.valueType()) {
                var colorNames = {
                    1: 'Light',
                    2: 'Medium',
                    3: 'Dark'
                };

                display = display + 'Color ';
                min = colorNames[min];
                max = colorNames[max];
            }

            if (self.range() === 'Minimum')
                display = display + min;
            else if (self.range() === 'Maximum')
                display = display + max;
            else if (self.range() === 'Both')
                display = display + min + ', ' + max;

            var enforcement = '';

            if (self.isEnforced() === 'true')
                enforcement = 'Enforced';
            else
                enforcement = 'Recommended';

            return display + ' (' + enforcement + ')'
        }, self);

        self.inEditMode = ko.observable(data === undefined);

        self.edit = function () {
            self.inEditMode(true);
        };

        self.cancel = function () {
            if (self.hasData()) {
                self.range(self.savedData.range);
                self.minValue(self.savedData.minValue);
                self.maxValue(self.savedData.maxValue);
                self.isEnforced(self.savedData.isEnforced);
                self.siteId(self.savedData.siteId);
                self.minDescription(self.savedData.minDescription);
                self.maxDescription(self.savedData.maxDescription);
                self.inEditMode(false);
            }
            else {
                self.remove(true);
            }
        };

        self.remove = function (ignoreWarning) {
            
            if (ignoreWarning) {
                parent.bounds.remove(self);
            }

            else {
                var message = 'To delete this range requirement, please type the word \"delete\" and click OK. Please note that these changes do not take effect until the system is saved.';
                util.promptForCommand(message, 'delete', function () {
                    parent.bounds.remove(self);
                }, function () {
                });
            }

        };

        self.isDirty = ko.computed(function () {
            return self.range() !== self.initialState.range
                || self.minValue() !== self.initialState.minValue
                || self.maxValue() !== self.initialState.maxValue
                || self.isEnforced() !== self.initialState.isEnforced
                || self.minDescription() !== self.initialState.minDescription
                || self.maxDescription() !== self.initialState.maxDescription
                || self.siteId() !== self.initialState.siteId;
        }, self);

        self.bake = function () {
            return {
                Id: self.savedData.id,
                Type: self.savedData.type,
                Range: self.savedData.range,
                MinValue: self.savedData.minValue || '',
                MaxValue: self.savedData.maxValue || '',
                MinDescription: self.savedData.minDescription,
                MaxDescription: self.savedData.maxDescription,
                IsEnforced: self.savedData.isEnforced,
                SiteId: self.savedData.siteId,
            };
        };

        self.isValid = ko.computed(function () {
            var errors = ko.validation.group([self.type, self.range, self.minValue, self.maxValue, self.isEnforced, self.siteId]);

            return errors().length === 0;
        });

        self.save = function () {

            var errors = ko.validation.group([self.type, self.range, self.minValue, self.maxValue, self.isEnforced, self.siteId]);

            if (errors().length > 0) {
                errors.showAllMessages();
                return;
            }

            if (self.showSite() === false) {
                self.siteId(0);
            }

            self.savedData.type = self.type();
            self.savedData.range = self.range();
            self.savedData.minValue = self.minValue();
            self.savedData.maxValue = self.maxValue();
            self.savedData.minDescription = self.minDescription();
            self.savedData.maxDescription = self.maxDescription();
            self.savedData.isEnforced = self.isEnforced();
            self.savedData.siteId = self.siteId();

            self.inEditMode(false);
            self.saveLabel('Update');
        };

    };

    return bounds;

})