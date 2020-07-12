// these are global so that the start date can be changed when the recurring shift controls are toggled on edit recurring shifts
function appendLeadingZeroes(n) {
    if (n <= 9) {
        return "0" + n;
    }
    return n;
}

function handleEventClick(shiftInfo) {

    let selectedShift = shiftInfo.event;

    // display recurring shift modal if the selected shift is a recurring shift
    let singleShiftModalMustBeDisplayed =
        typeof selectedShift._def.recurringDef == "undefined" ||
        typeof selectedShift._def.recurringDef == "";

    if (singleShiftModalMustBeDisplayed) {
        populateSingleShiftModal(selectedShift);

        $('#edit-shift-modal').modal();
    }
    else {
        populateRecurringShiftModal(selectedShift);

        $('#edit-recshift-modal').modal();

        // resets the modal
        $('#edit-recshift-display-single').prop('checked', true);
        $('#edit-recshift-display-all').prop('checked', false);
        toggleEditRecurringShiftControls();
    }
}