let originalEntryValues;

function makeEntryEditable(id) {

    originalEntryValues = {
        volunteer: document.getElementById("entry-volunteer-" + id).value,
        position: document.getElementById("entry-position-" + id).value,
        date: document.getElementById("entry-date-" + id).value,
        start: document.getElementById("entry-starttime-" + id).value,
        end: document.getElementById("entry-endtime-" + id).value
    }

    let volunteerField = document.getElementById("entry-volunteer-" + id);
    let positionField = document.getElementById("entry-position-" + id);
    let dateField = document.getElementById("entry-date-" + id);
    let startField = document.getElementById("entry-starttime-" + id);
    let endField = document.getElementById("entry-endtime-" + id);

    let entryFields = [];

    entryFields.push(volunteerField);
    entryFields.push(positionField);
    entryFields.push(dateField);
    entryFields.push(startField);
    entryFields.push(endField);

    entryFields.forEach(function (field) {
        if (field.nodeName == "INPUT") {
            field.readOnly = false;
        }
        if (field.nodeName == "SELECT") {
            field.disabled = false;
        }

        field.classList.add("being-edited");
    })

    let editButtons = Array.from(document.getElementsByClassName("entry-edit-button"));
    let deleteButtons = Array.from(document.getElementsByClassName("entry-delete-button"));

    let defaultButtons = editButtons.concat(deleteButtons);

    defaultButtons.forEach(function (button) {
        button.style.display = "none";
    })

    document.getElementById("entry-save-" + id).style.display = "inline";
    document.getElementById("entry-cancel-" + id).style.display = "inline";
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

    let editButtons = Array.from(document.getElementsByClassName("entry-edit-button"));
    let deleteButtons = Array.from(document.getElementsByClassName("entry-delete-button"));

    let defaultButtons = editButtons.concat(deleteButtons);

    defaultButtons.forEach(function (button) {
        button.style.display = "inline";
    })

    document.getElementById("entry-save-" + id).style.display = "none";
    document.getElementById("entry-cancel-" + id).style.display = "none";
}