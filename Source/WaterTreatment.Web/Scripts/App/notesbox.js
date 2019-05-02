'use strict';
define(['jquery', 'knockout', 'hasFocus'], function ($, ko) {
    return function (options) {
        var self = this;
        self.readOnlyLabel = options.readOnlyLabel;
        self.preserveReadOnlyFormat = options.preserveReadOnlyFormat;
        self.mainPlaceholderText = options.mainPlaceholderText;
        self.secondaryPlaceholderText = options.secondaryPlaceholderText;
        self.isEnteringNotesText = options.isEnteringNotesText;
        self.saveLabel = options.saveLabel || 'OK';
        self.undoLabel = options.undoLabel || 'Undo';
        self.editLabel = options.editLabel || 'Edit';
        self.cancelLabel = options.cancelLabel || 'Cancel';
        self.hideEdit = options.hideEdit;
        self.hideUndo = options.hideUndo;
        self.notes = ko.observable(options.initialNotes);

        self.disableActions = ko.observable(false);

        self.notesSaved = ko.computed(function () {
            var notes = self.notes();

            return notes !== undefined && notes !== null && notes.length > 0;
        });

        self.notesEntered = ko.observable();
        self.notesHasFocus = ko.observable();
        self.isEnteringNotes = ko.observable(false);
        self.notesHasFocus.subscribe(function (nhf) {
            if (nhf) {
                self.isEnteringNotes(true);
            }
        });

        self.cancel = function () {
            self.notesHasFocus(false);
            self.isEnteringNotes(false);
            self.notesEntered('');
            self.notes('');
        };

        self.save = function () {
            self.notes(self.notesEntered());
            self.notesHasFocus(false);
            self.isEnteringNotes(false);
            if (options.onSaved) options.onSaved();
        };

        self.undo = function () {
            self.notesEntered('');
            self.notes(null);
        };

        self.edit = function () {
            self.notesEntered(self.notes());
            self.notes(null);
            self.notesHasFocus(true);
        };
    };
});