let originalEntryValues;

function makeEntryEditable(id, isCurrent) {

    originalEntryValues = {
        volunteer: document.getElementById("entry-volunteer-" + id).value,
        position: document.getElementById("entry-position-" + id).value,
        startDate: document.getElementById("entry-startdate-" + id).value,
        startTime: document.getElementById("entry-starttime-" + id).value,
    }

    if (!isCurrent) {
        originalEntryValues.endDate = document.getElementById("entry-enddate-" + id).value
        originalEntryValues.endTime = document.getElementById("entry-endtime-" + id).value

    }

    let volunteerField = document.getElementById("entry-volunteer-" + id);
    let positionField = document.getElementById("entry-position-" + id);
    let startDateField = document.getElementById("entry-startdate-" + id);
    let startTimeField = document.getElementById("entry-starttime-" + id);
    let endDateField = document.getElementById("entry-enddate-" + id);
    let endTimeField = document.getElementById("entry-endtime-" + id);

    let entryFields = [];

    entryFields.push(volunteerField);
    entryFields.push(positionField);
    entryFields.push(startDateField);
    entryFields.push(startTimeField);
    if (!isCurrent) {
        entryFields.push(endDateField);
        entryFields.push(endTimeField);
    }

    entryFields.forEach(function (field) {
        if (field.nodeName == "INPUT") {
            field.readOnly = false;
        }
        if (field.nodeName == "SELECT") {
            field.disabled = false;
        }

        field.classList.add("being-edited");
    })

    let unselectedButtons = Array.from(document.getElementsByClassName("unselected-buttons"));

    unselectedButtons.forEach(function (buttonGroup) {
        buttonGroup.style.display = "none";
    })

    document.getElementById("entry-selected-" + id).style.display = "inline-flex";
}

function cancelEntryEdit(id) {

    let entryFields = Array.from(document.getElementsByClassName("being-edited"));

    entryFields.forEach(function (field) {
        if (field.nodeName == "INPUT") {
            field.readOnly = true;
        }
        if (field.nodeName == "SELECT") {
            field.disabled = true;
        }

        field.classList.remove("being-edited");
    })

    document.getElementById("entry-volunteer-" + id).value = originalEntryValues.volunteer;
    document.getElementById("entry-position-" + id).value = originalEntryValues.position
    document.getElementById("entry-startdate-" + id).value = originalEntryValues.startDate;
    document.getElementById("entry-starttime-" + id).value = originalEntryValues.startTime;
    document.getElementById("entry-enddate-" + id).value = originalEntryValues.endDate;
    document.getElementById("entry-endtime-" + id).value = originalEntryValues.endTime;

    let unselectedButtons = Array.from(document.getElementsByClassName("unselected-buttons"));

    unselectedButtons.forEach(function (button) {
        button.style.display = "inline-flex";
    })

    document.getElementById("entry-selected-" + id).style.display = "none";
}