define(['knockout', 'App/ViewModels/System/bounds', 'regex-weburl', 'App/compareEntities', 'util', 'koVal'], function (ko, bound, urlRE, compareEntities, util) {

    var parameter = function (parent, parameterTypes, data) {

        var self = this;

        self.parent = parent;
        self.parameterTypes = parameterTypes;

        self.initialState = {
            id: 0,
            name: '',
            frequency: '',
            parameterTypeId: 1,
            unit: '',
            source: '',
            link: '',
            use: 'Measurement',
            bounds: []
        };

        self.savedData = {
            id: 0,
            name: '',
            frequency: '',
            parameterTypeId: 1,
            unit: '',
            source: '',
            link: '',
            use: 'Measurement',
            bounds: []
        };

        self.saveLabel = ko.observable('Add');

        if (typeof data !== 'undefined') {
            self.initialState.id = data.Id;
            self.initialState.name = data.Name;
            self.initialState.frequency = data.Frequency;
            self.initialState.parameterTypeId = data.ParameterTypeId;
            self.initialState.unit = data.Unit;
            self.initialState.source = data.Source;
            self.initialState.link = data.Link;
            self.initialState.use = data.Use || 'Measurement';
            self.initialState.bounds = data.Bounds.map(function (b) { return new bound(self, b); });
            
            self.savedData.id = data.Id;
            self.savedData.name = data.Name;
            self.savedData.frequency = data.Frequency;
            self.savedData.parameterTypeId = data.ParameterTypeId;
            self.savedData.unit = data.Unit;
            self.savedData.source = data.Source;
            self.savedData.link = data.Link;
            self.savedData.use = data.Use || 'Measurement'
            self.savedData.bounds = data.Bounds.map(function (b) { return new bound(self, b); });

            self.saveLabel('Update');
        }

        self.name = ko.observable(self.savedData.name).extend({ required: true });
        self.frequency = ko.observable(self.savedData.frequency).extend({ required: true });
        self.parameterTypeId = ko.observable(self.savedData.parameterTypeId).extend({ required: true });
        self.unit = ko.observable(self.savedData.unit).extend({ required: true });
        self.source = ko.observable(self.savedData.source).extend({ required: true });
        self.link = ko.observable(self.savedData.link).extend({
            pattern: {
                message: "Not a valid URL.",
                params: urlRE
            }
        });
        self.use = ko.observable(self.savedData.use).extend({ required: true });

        self.bounds = ko.observableArray(self.savedData.bounds);

        self.displayType = ko.computed(function () {
            var pId = self.parameterTypeId();
            return parameterTypes.filter(function (p) { return p.Id == pId }).pop().Name;
        }, self);

        self.hasData = function () {
            return self.savedData.name !== '';
        };

        self.inEditMode = ko.observable(data === undefined);

        self.areBoundsDirty = ko.computed(function () {
            return self.bounds().some(function (b) { return b.isDirty(); });
        }, self);

        self.isDirty = ko.computed(function () {

            var name = self.name();
            var frequency = self.frequency();
            var parameterTypeId = self.parameterTypeId();
            var unit = self.unit();
            var source = self.source();
            var link = self.link();
            var areBoundsDirty = self.areBoundsDirty();
            var boundsChanged = !compareEntities(self.initialState.bounds, self.bounds());

            return name !== self.initialState.name
                || frequency !== self.initialState.frequency
                || parameterTypeId !== self.initialState.parameterTypeId
                || unit !== self.initialState.unit
                || source !== self.initialState.source
                || link !== self.initialState.link
                || areBoundsDirty
                || boundsChanged;
        }, self);

        self.edit = function() {
            self.inEditMode(true);
        };

        self.addBound = function () {
            var b = new bound(self);
            self.bounds.push(b);
        };

        self.remove = function (ignoreWarning) {

            if (ignoreWarning) {
                parent.parameters.remove(self);
            }

            else {
                var message = 'To delete this parameter, please type the word \"delete\" and click OK. Please note that these changes do not take effect until the system is saved.';
                util.promptForCommand(message, 'delete', function () {
                    parent.parameters.remove(self);
                }, function () {
                });
            }

        };

        self.cancel = function () {
            if (self.hasData()) {
                self.name(self.savedData.name);
                self.frequency(self.savedData.frequency);
                self.parameterTypeId(self.savedData.parameterTypeId);
                self.unit(self.savedData.unit);
                self.source(self.savedData.source);
                self.link(self.savedData.link);
                self.bounds(self.savedData.bounds);
                self.use(self.savedData.use);
                self.inEditMode(false);
            }
            else {
                self.remove(true);
            }
        };

        self.anyEditing = ko.computed(function () {

            var boundsEditing = self.bounds().some(function (b) { return b.inEditMode(); });
            return self.inEditMode() || boundsEditing;
        }, self);

        self.bake = function () {
            return {
                Id: self.savedData.id,
                Name: self.savedData.name,
                Frequency: self.savedData.frequency,
                ParameterTypeId: self.savedData.parameterTypeId,
                Unit: self.savedData.unit,
                Source: self.savedData.source,
                Link: self.savedData.link,
                Use: self.savedData.use,
                Bounds: self.savedData.bounds.map(function (b) { return b.bake(); })
            };
        };

        self.isValid = ko.computed(function () {
            var errors = ko.validation.group([self.name, self.frequency, self.parameterTypeId, self.unit, self.source, self.link, self.use]);

            return errors().length === 0 && self.bounds().reduce(function (val, bound) { return val && bound.isValid(); }, true);
        });

        self.save = function () {
            var errors = ko.validation.group([self.name, self.frequency, self.parameterTypeId, self.unit, self.source, self.link, self.use]);

            if (errors().length > 0) {
                errors.showAllMessages();
                return;
            }

            self.savedData.name = self.name();
            self.savedData.frequency = self.frequency();
            self.savedData.parameterTypeId = self.parameterTypeId();
            self.savedData.unit = self.unit();
            self.savedData.source = self.source();
            self.savedData.link = self.link();
            self.savedData.use = self.use();
            self.savedData.bounds = self.bounds();

            self.inEditMode(false);
            self.saveLabel('Update');

        };

    };

    return parameter;

})