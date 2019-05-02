define(['knockout', 'jquery', 'App/ViewModels/DataEntry/measurement', 'App/ViewModels/DataEntry/adHocmeasurement', 'notesBox', 'App/webservice'], function (ko, $, measurementVM, ahmeasurementVM, notesBox, webService) {

    return function (data, isProcessingRequestObs) {

        var self = this;

        self.id = ko.observable(data.Id);
        self.location = ko.observable(data.Location);
        self.description = ko.observable(data.Description);
        self.systemName = ko.observable(data.SystemName);

        self.initialReason = data.ReasonSkipped;

        self.measurements = ko.observableArray(data.Measurements.map(function (m) { return new measurementVM(m); }));

        self.collapseId = ko.computed(function () {

            return 'collapsible' + self.id();

        }, self);

        self.collapseAnchor = ko.computed(function () {

            return '#' + self.collapseId();

        }, self);

        self.isCollapsed = ko.observable();

        self.hide = function () {
            $(self.collapseAnchor()).collapse('hide')
        };

        self.bake = function () {
            return {
                Id: self.id(),
                Location: self.location(),
                Description: self.description(),
                SystemName: self.systemName(),
                ReasonSkipped: self.skipNotesBox.notes() || '',
                Measurements: self.measurements().map(function (m) { return m.bake(); }),
            };
        };

        self.skipNotesBox = new notesBox({
            initialNotes: data.ReasonSkipped,
            readOnlyLabel: 'Reason Skipped',
            preserveReadOnlyFormat: false,
            mainPlaceholderText: 'Want to skip this water system?',
            secondaryPlaceholderText: 'Write your reasons.',
            isEnteringNotesText: 'This report cannot be saved while trying to skip a measurement. Please confirm or cancel this action.',
            saveLabel: 'Skip',
            undoLabel: 'Undo Skip',
            hideUndo: false,
            hideEdit: true,
            onSaved: self.hide,
        });

        isProcessingRequestObs.subscribe(function (ipr) {
            self.skipNotesBox.disableActions(ipr);
        });

        self.displayStatuses = ko.computed(function () {

            var statuses = [];


            if (self.measurements().some(function (m) { return m.isComplete() && m.isApplicable(); }))
                statuses.push('Complete');

            if (!self.skipNotesBox.notesSaved() && self.measurements().some(function (m) { return !m.isComplete(); }))
                statuses.push('Incomplete');

            if (self.skipNotesBox.notesSaved() || self.measurements().some(function (m) { return !m.isApplicable(); }))
                statuses.push('Skipped');

            if (self.measurements().some(function (m) {

                var value = m.value();
                var isInvalid = m.isInvalid();

                return (m.immediatelyValidate || !m.emptyCheck(value)) && isInvalid;
            }))
                statuses.push('Invalid');

            return statuses;
        }, self);

        self.addAdhoc = function (adhoc) {
            self.measurements.push(new ahmeasurementVM(adhoc));
        };

        self.removeAdhoc = function (adhoc) {
            var r = confirm("Are you sure you want to delete this measurement?.");
            if (r == true) {
                var id = adhoc.id;
                if (id._latestValue > 0) {
                    webService.Get('/DataEntry/RemoveAdhoc/', { 'id': id }).then(function (data) {
                        ;
                    });
                }
                self.measurements.remove(adhoc);
            }
            
        };


        //Not sure why subscribe on self.status wasn't firing initially.
        ko.computed(function () {

            var canEdit = self.skipNotesBox.notesSaved() ? false : true;

            self.measurements().forEach(function (measurement) { measurement.canEdit(canEdit); });
        });

        self.canSave = ko.computed(function () {

            var reasonSaved = self.skipNotesBox.notesSaved();

            return self.measurements().every(function (m) {
                var isComplete = m.isComplete();
                return isComplete || reasonSaved;
            });

        }, self);

        self.isDirty = ko.computed(function () {

            var reasonChanged = self.initialReason !== self.skipNotesBox.notes() || (self.skipNotesBox.isEnteringNotes() && typeof self.skipNotesBox.notesEntered() == 'string' && self.skipNotesBox.notesEntered() !== '' && self.skipNotesBox.notesEntered() !== self.initialReason);
            var anyMeasurementsChanged = self.measurements().some(function (m) { return m.isDirty(); });

            return reasonChanged || anyMeasurementsChanged;
        }, self);

    };

});