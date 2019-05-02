define(['knockout', 'App/ViewModels/DataEntry/commentDialog', 'koVal'], function (ko, commentDialog) {

    return function (data, measurementCommentVM) {

        var self = this;

        self.id = ko.observable(data.Id);
        self.name = ko.observable(data.Name);
        self.source = ko.observable(data.Source);
        self.hasBounds = ko.observable(data.HasBounds);
        self.type = ko.observable(data.Type || 'Value');
        self.unit = ko.observable(data.Unit);
        self.range = ko.observable(data.Range);
        self.minValue = ko.observable(data.MinValue).extend({ pattern: '^[a-z0-9].$' });
        self.maxValue = ko.observable(data.MaxValue).extend({ pattern: '^[a-z0-9].$' });
        self.isEnforced = ko.observable(data.IsEnforced);
        self.comment = ko.observable(data.Comment);
        self.isAdhoc = ko.observable(data.IsAdhoc);
        self.Delete = ko.observable(data.Delete);

        self.emptyCheck = function (val) {
            return val === undefined
                || val === null
                || val.trim() === '';
        };

        var initial = data.Value;
        self.immediatelyValidate = true;

        if (initial === undefined || initial === null || initial === '') {
            self.immediatelyValidate = false;
            initial = self.type() === 'Value' ? '' : self.minValue();
        }

        self.initialState = initial;

        self.value = ko.observable(initial).extend({ number: true });

        self.isApplicable = ko.observable(data.IsApplicable);

        //self.hide = ko.computed(function () {
        //    var isProtableWaterSyetem = $parent

        //}
        
        //);

        self.isDirty = ko.computed(function () {
            return self.value() !== self.initialState || (data.IsApplicable !== self.isApplicable()) || (data.Comment !== self.comment());
        }, self);

        self.canEdit = ko.observable(true);

        self.isSkipped = ko.computed(function () {
            var canEdit = self.canEdit();
            var isApplicable = self.isApplicable();
            return canEdit && isApplicable;
        });

        self.isEmpty = ko.computed(function () {
            return self.emptyCheck(self.value());
        }, self);

        self.value.extend({ required: true }).extend({ number: true });

        if (self.immediatelyValidate) {
            ko.validation.group([self.value]).showAllMessages(true);
        }

        self.valueType = ko.computed(function () {
            return self.type() === 'Value';
        }, self);

        self.isInvalid = ko.computed(function () {
            var errors = ko.validation.group([self.value]);
            if (errors().length > 0) {
                return true;
            }
        });

        self.showError = ko.computed(function () {

            var value = self.value();
            var isInvalid = self.isInvalid();
            var isApplicable = self.isApplicable();

            return (self.immediatelyValidate || !self.emptyCheck(value)) && isInvalid && isApplicable;;
        });

        self.showErrorMax = ko.computed(function () {
            var value = self.maxValue();
            var isInvalid = isNaN(value);
            return  isInvalid && (self.immediatelyValidate || !self.emptyCheck(value)) ;
        });

        self.showErrorMin = ko.computed(function () {
            var value = self.minValue();
            var isInvalid = isNaN(value);
            return isInvalid && (self.immediatelyValidate || !self.emptyCheck(value));
        });

        self.isComplete = ko.computed(function () {

            var isInvalid = self.isInvalid();
            var isEmpty = self.emptyCheck(self.value());
            var isApplicable = self.isApplicable();

            if (!isApplicable) {
                return true;
            }

            if (self.isInvalid()) {
                return false;
            }

            return !isEmpty;

        }, self);

        self.icon = ko.computed(function () {

            var hasComments = !self.emptyCheck(self.comment());

            var actions = hasComments ? 'actions-hascomments' : 'actions-nocomments';
            var icon = '';

            if (hasComments)
                icon = 'fa-commenting';
            else
                icon = 'fa-comment-o'

            return 'fa ' + icon + ' fa-fw ' + actions;
        });

        self.skip = ko.computed(function () {
            if (!self.isApplicable()) {
                return 'checkbox-slider--b-flat data-entry-actions-unskip';
            }
            return 'checkbox-slider--b-flat data-entry-actions-skip';
        });

        self.showCommentDialog = function () {
            commentDialog.display(self.comment);
        };

        self.textClass = ko.computed(function () {
            return self.isApplicable() ? '' : 'text-disabled'
        });

        self.bake = function () {
            return {
                Id: self.id(),
                Name: self.name(),
                Source: self.source(),
                HasBounds: self.hasBounds(),
                Type: self.type(),
                Range: self.range(),
                IsEnforced: self.isEnforced(),
                IsAdhoc: self.isAdhoc(),
                MinValue: self.minValue(),
                MaxValue: self.maxValue(),
                Unit: self.unit(),
                Value: self.value(),
                IsApplicable: self.isApplicable(),
                Comment: self.comment(),
            };
        };

    };

});