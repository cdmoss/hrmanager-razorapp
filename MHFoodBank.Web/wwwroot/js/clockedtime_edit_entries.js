function makeEntryEditable(id) {
    const volunteer = document.getElementById("entry-volunteer-" + id);
    const originalNameId = volunteer.id;

    volunteer.name = "EditVolunteerId";
    volunteer.disabled = false;

    document.getElementById("save-entry-" + id).style.display = "inline";
}