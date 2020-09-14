// open shifts
function setShiftAsOpenAdd() {
    var markedAsOpen = $('#open-shift-add').prop('checked');
    var volunteerContainer = document.getElementById('volunteer-container-add');
    var volunteerField = document.getElementById('add-shift-volunteer');
    if (markedAsOpen == true) {
        volunteerField.disabled = true;
        volunteerContainer.style.display = "none";
    } else {
        volunteerField.disabled = false;
        volunteerContainer.style.display = "inline";
    }
}

function setShiftAsOpenEdit() {
    var markedAsOpen = $('#open-shift-edit').prop('checked');
    var volunteerContainer = document.getElementById('volunteer-container-edit');
    var volunteerField = document.getElementById('edit-shift-volunteer');
    if (markedAsOpen == true) {
        volunteerField.disabled = true;
        volunteerContainer.style.display = "none";
    } else {
        volunteerField.disabled = false;
        volunteerContainer.style.display = "inline";
    }
}

function setShiftAsMultipleAdd() {
    var markedAsMultiple = $('#multiple-shift-add').prop('checked');
    var positionContainer = document.getElementById('position-container');
    var positionField = document.getElementById('add-shift-position');
    var multipleContainer = document.getElementById('multiple-shifts-container');
    if (markedAsMultiple == true) {
        positionField.disabled = true;
        positionContainer.style.display = "none";
        Array.from(multipleContainer.getElementsByTagName('INPUT')).forEach(function (input) {
            input.disabled = false;
        })
        multipleContainer.style.display = "block"; 
        if ($("#add-recshift-display").prop("checked")) {
            $('#add-manyrecshift-submit').prop('style', 'display: inline');
            $('#add-manyshift-submit').prop('style', 'display: none');
            $('#add-shift-submit').prop('style', 'display: none');
            $('#add-recshift-submit').prop('style', 'display: none');
        }
        else {
            $('#add-manyrecshift-submit').prop('style', 'display: none');
            $('#add-manyshift-submit').prop('style', 'display: inline');
            $('#add-shift-submit').prop('style', 'display: none');
            $('#add-recshift-submit').prop('style', 'display: none');
        }

    } else {
        positionField.disabled = false;
        positionContainer.style.display = "inline";
        Array.from(multipleContainer.getElementsByTagName('INPUT')).forEach(function (input) {
            input.disabled = true;
        })
        multipleContainer.style.display = "none"
        if ($("#add-recshift-display").prop("checked")) {
            $('#add-manyrecshift-submit').prop('style', 'display: none');
            $('#add-manyshift-submit').prop('style', 'display: none');
            $('#add-shift-submit').prop('style', 'display: none');
            $('#add-recshift-submit').prop('style', 'display: inline');
        }
        else {
            $('#add-manyrecshift-submit').prop('style', 'display: none');
            $('#add-manyshift-submit').prop('style', 'display: none');
            $('#add-shift-submit').prop('style', 'display: inline');
            $('#add-recshift-submit').prop('style', 'display: none');
        }
    }
}

function setRecurringShiftAsOpen() {
    var markedAsOpen = $('#open-shift-edit-rec').prop('checked');
    var volunteerContainer = document.getElementById('volunteer-container-edit-rec');
    var volunteerField = document.getElementById('edit-recshift-volunteer');
    if (markedAsOpen == true) {
        volunteerField.disabled = true;
        volunteerContainer.style.display = "none";
    } else {
        volunteerField.disabled = false;
        volunteerContainer.style.display = "inline";
    }
}

// for changing the modal display based on recurring shift checkbox
function toggleAddRecurringShiftControls() {
    if ($("#add-recshift-display").prop("checked") == true) {
        const startDate = moment(document.getElementById("add-shift-date").value);
        // shows recurring shift controls
        $('#add-recshift-container').prop('style', 'display: block');
        // shows add recurring shift button, hides add shift button
        if ($("#multiple-shift-add").prop("checked") == true) {
            $('#add-manyrecshift-submit').prop('style', 'display: inline');
            $('#add-manyshift-submit').prop('style', 'display: none');
            $('#add-shift-submit').prop('style', 'display: none');
            $('#add-recshift-submit').prop('style', 'display: none');
        }
        else {
            $('#add-manyrecshift-submit').prop('style', 'display: none');
            $('#add-manyshift-submit').prop('style', 'display: none');
            $('#add-shift-submit').prop('style', 'display: none');
            $('#add-recshift-submit').prop('style', 'display: inline');
        }
        $('#add-recshift-enddate').prop('required', true);
        $('#add-recshift-enddate').val(startDate.add(7, 'days').format('yyyy-MM-DD'));
        $('.datepair').datepair();
    }
    else if ($("#add-recshift-display").prop("checked") == false) {
        // hides recurring shift controls
        $('#add-recshift-container').prop('style', 'display: none');
        if ($("#multiple-shift-add").prop("checked") == true) {
            $('#add-manyrecshift-submit').prop('style', 'display: none');
            $('#add-manyshift-submit').prop('style', 'display: inline');
            $('#add-shift-submit').prop('style', 'display: none');
            $('#add-recshift-submit').prop('style', 'display: none');
        }
        else {
            $('#add-manyrecshift-submit').prop('style', 'display: none');
            $('#add-manyshift-submit').prop('style', 'display: none');
            $('#add-shift-submit').prop('style', 'display: inline');
            $('#add-recshift-submit').prop('style', 'display: none');
        }
        $('#add-recshift-enddate').prop('required', false);
        $('.datepair').datepair('remove');
    }
}

function toggleEditRecurringShiftControls() {
    var markedAsSelectedShift = $('#edit-recshift-display-single').prop('checked');
    var startDateLabel = document.getElementById('edit-recshift-startdate-label');
    if (markedAsSelectedShift) {
        $('#edit-recshift-container').prop('style', 'display: none');
        $('#edit-recshift-single-initial-startdate').prop('style', 'display: inline-flex');
        $('#edit-recshift-all-startdate').prop('style', 'display: none');
        $('#edit-recshift-enddate').prop('required', false);
        $('.datepair').datepair();
        startDateLabel.innerHTML = "Selected Shift Date"
    }
    else if (markedAsSelectedShift == false) {
        $('#edit-recshift-container').prop('style', 'display: block');
        $('#edit-recshift-single-date').prop('style', 'display: none');
        $('#edit-recshift-all-date').prop('style', 'display: inline-flex');
        $('#edit-recshift-enddate').prop('required', true);
        $('.datepair').datepair('remove');
        startDateLabel.innerHTML = "Recurring Shift Start Date"
    }
}