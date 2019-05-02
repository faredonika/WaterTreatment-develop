define(['jquery', 'knockout', 'App/ViewModels/DataEntry/reportBuilding', 'App/webservice', 'App/ViewModels/DataEntry/attachments', 'moment', 'App/ViewModels/DataEntry/commentDialog', 'notesBox', 'datePicker'], function ($, ko, buildingVM, webService, attachmentVM, moment, commentDialog, notesBox) {

    return function (data) {

        var self = this;
        var dateFormat = 'M/D/YYYY';

        self.commentDialog = commentDialog;
        self.isProcessingRequest = ko.observable(false);

        self.id = ko.observable(data.Id);
        self.buildings = ko.observableArray(data.Buildings.map(function (b) { return new buildingVM(b, self.isProcessingRequest); }));
        self.attachments = new attachmentVM({ Id: data.Id });
        self.measurementDate = ko.observable().extend({ required: true });

        self.initialMeasurementDate = moment(data.MeasurementDate);
        if (self.initialMeasurementDate.isValid()) {
            self.measurementDate(self.initialMeasurementDate.format(dateFormat));
        }

        self.isSaving = ko.observable(false);
        self.pendingNotes = ko.observable();
        self.isEnteringNotes = ko.observable(false);

        self.reportNotes = new notesBox({
            initialNotes: data.Notes || '',
            readOnlyLabel: undefined,
            preserveReadOnlyFormat: true,
            mainPlaceholderText: 'Want to add additional information to this report?',
            secondaryPlaceholderText: 'Write your notes.',
            isEnteringNotesText: 'This report cannot be saved while trying to add notes. Please confirm or cancel this action.',
            saveLabel: 'OK',
            editLabel: 'Edit Notes',
            hideUndo: true,
            hideEdit: false,
            onSaved: self.hide,
        });

        self.isProcessingRequest.subscribe(function (ipr) {
            self.reportNotes.disableActions(ipr);
        });

        self.confirmExit = function () {
            if (self.isDirty() && !self.isSaving()) {
                self.isSaving(false);
                return 'You have unsaved changes. Are you sure you wish to leave this page?';
            }
        };

        self.mouseOut = function () {
            window.onbeforeunload = self.confirmExit;
            return true;
        };

        self.ClickAttachment = function () {
            window.onbeforeunload = null;
            return true;
        };

        self.canSave = ko.computed(function () {
            return self.measurementDate.isValid() && !self.isProcessingRequest() && !self.reportNotes.isEnteringNotes() && self.buildings().reduce(function (canSave, building) {
                return building.systems().reduce(function (canSave, system) {
                    return canSave && !system.skipNotesBox.isEnteringNotes();
                }, canSave);
            }, true);
        });

        self.canSubmit = ko.computed(function () {
            var canSave = self.canSave();
            var buildingsCanSubmit = self.buildings().every(function (b) { return b.canSubmit(); });
            return canSave && buildingsCanSubmit && self.attachments.list().length > 0;
        }, self);

        self.bake = function () {
            return {
                Id: self.id(),
                MeasurementDate: moment(self.measurementDate()).format(dateFormat),
                Buildings: self.buildings().map(function (b) { return b.bake(); }),
                Notes: self.reportNotes.notes()
            };
        };

        self.isDirty = ko.computed(function () {

            // bootstrap datepicker creates a new date object in our observable so convert back to a date string
            var measurementChanged = self.initialMeasurementDate.isValid() && self.initialMeasurementDate.format(dateFormat) !== moment(self.measurementDate()).format(dateFormat);
            var anyBuildingsChanged = self.buildings().some(function (b) { return b.isDirty(); });
            var reasonChanged = (self.reportNotes.notes() !== '' && data.Notes !== self.reportNotes.notes()) || (self.reportNotes.isEnteringNotes() && self.reportNotes.notesEntered() !== '' && self.reportNotes.notesEntered() !== data.Notes);

            return measurementChanged || anyBuildingsChanged || reasonChanged;
        }, self);

        self.save = function () {
            if (self.isProcessingRequest()) return;
            self.isProcessingRequest(true);

            self.isSaving(true);

            var errors = ko.validation.group([self.measurementDate]);

            if (errors().length > 0) {
                errors.showAllMessages();
                self.isProcessingRequest(false);
                return;
            }

            var url = '/DataEntry/Edit/' + self.id();
            webService.SubmitFormCSRF(url, self.bake());

        };

        self.submit = function () {
            if (self.isProcessingRequest()) return;
            self.isProcessingRequest(true);
            var errors = ko.validation.group([self.measurementDate]);

            self.isSaving(true);

            if (errors().length > 0) {
                errors.showAllMessages();
                self.isProcessingRequest(false);
                return;
            }

            var url = '/DataEntry/EditSubmit/';
            webService.SubmitFormCSRF(url, self.bake());

        };

        //self.removeAdhocRt = function (adhoc) {
        //    var r = confirm("Are you sure you want to delete this measurement?.");
        //    if (r == true) {
        //        var id = adhoc.id;
        //        self.building[0].systems[0].measurements[0].remove[adhoc];
        //        webService.Get('/DataEntry/RemoveAdhoc/', { 'id': id }).then(function (data) {
        //            ;
        //        });
        //    }
        //};
    }
});