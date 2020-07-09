// these are global so that the start date can be changed when the recurring shift controls are toggled on edit recurring shifts
function appendLeadingZeroes(n) {
    if (n <= 9) {
        return "0" + n;
    }
    return n;
}

function handleEventClick(shiftInfo) {

    let selectedShift = shiftInfo.event;
    let selectedShiftWasPartOfRecurringSet = selectedShift.extendedProps.wasPartOfRecurring.length !== 0;

    // display recurring shift modal if the selected shift is a recurring shift
    let singleShiftModalMustBeDisplayed =
        typeof selectedShift._def.recurringDef == "undefined" ||
        typeof selectedShift._def.recurringDef == "";

    // determine if shift is open
    let shiftIsOpen = selectedShift.extendedProps.vol - 1 < 0

    if (singleShiftModalMustBeDisplayed) {

        $('#edit-shift-modal').modal();
        selectedShiftStartDate = new Date(selectedShift.start);
        selectedShiftEndDate = new Date(selectedShift.end);
        document.getElementById("edit-shift-id").value = selectedShift.id;
        document.getElementById("edit-shift-date").value = selectedShiftInitialStartDate.getFullYear() + "-" + appendLeadingZeroes(selectedShiftInitialStartDate.getMonth() + 1) + "-" + appendLeadingZeroes(selectedShiftInitialStartDate.getDate());
        document.getElementById("edit-shift-starttime").value = appendLeadingZeroes(selectedShiftInitialStartDate.getHours()) + ":" + appendLeadingZeroes(selectedShiftInitialStartDate.getMinutes());
        document.getElementById("edit-shift-endtime").value = appendLeadingZeroes(select.getHours()) + ":" + appendLeadingZeroes(selectedShiftEndDate.getMinutes());
        document.getElementById("edit-shift-position").selectedIndex = (shiftInfo.event.extendedProps.posWorked - 1);

        let volunteerId = selectedShift.extendedProps.vol - 1;

        if (shiftIsOpen) {
            document.getElementById("open-shift-edit").checked = true;
            setShiftAsOpenEdit();
        }

        else {
            document.getElementById("open-shift-edit").checked = false;
            document.getElementById("edit-shift-volunteer").selectedIndex = volunteerId;
        }

        if (wasPartOfRecurringSet) {
            $('#edit-shift-delete-prompt').prop('style', 'display: block');
            $('#edit-shift-delete-noprompt').prop('style', 'display: none');
            document.getElementById('edit-recshift-single-confirm-id').value = selectedShift.id
        }
        else {
            $('#edit-shift-delete-prompt').prop('style', 'display: none');
            $('#edit-shift-delete-noprompt').prop('style', 'display: block');
        }
    }
    else {
        populateRecurringShiftModal(selectedShift);

        $('#edit-recshift-modal').modal();

        if (selectedShiftWasPartOfRecurringSet) {
            // display the recurring shift modal with the single shift controls

        }


        // resets the modal
        $('#edit-recshift-display-single').prop('checked', true);
        $('#edit-recshift-display-all').prop('checked', false);
        toggleEditRecurringShiftControls();

        if (shiftIsOpen) {
            document.getElementById("open-shift-edit-rec").checked = true;
            setRecurringShiftAsOpen();
        }
        else {
            document.getElementById("edit-recshift-volunteer").selectedIndex = shiftInfo.event.extendedProps.vol - 1;
        }
    }
}