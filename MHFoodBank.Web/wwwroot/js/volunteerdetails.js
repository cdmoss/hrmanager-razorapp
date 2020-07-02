document.onload = function () {
    initializeWorkExperiences();

    function initializeWorkExperiences() {
        let checkboxes = $('[id^="work-exp-current"]');
        for (var i = 0; i < checkboxes.length; i++) {
            if (checkboxes[i].checked === true) {
                let checkedId = checkboxes[i].id[checkboxes[i].id.length - 1];
                let workExperienceEndDateId = "workexp-current-enddate-" + checkedId;
                let workExperienceLabelId = "workexp-current-label-" + checkedId;
                let workExperienceEndDate = document.getElementById(workExperienceEndDateId);
                let workExperienceLabel = document.getElementById(workExperienceLabelId);
                workExperienceEndDate.style.display = 'none';
                workExperienceLabel.style.display = 'none';
            }
        }
    }

    makeWorkExperienceCurrent();

    function makeWorkExperienceCurrent(workExpId) {

        // get the ids of the selected work experience elements
        let workExperienceCheckId = "workexp-current-check-" + workExpId;
        let workExperienceEndDateId = "workexp-current-enddate-" + workExpId;
        let workExperienceLabelId = "workexp-current-label-" + workExpId;

        // get the elements using the ids
        let workExperienceCheck = document.getElementById(workExperienceCheckId);
        let workExperienceEndDate = document.getElementById(workExperienceEndDateId);
        let workExperienceLabel = document.getElementById(workExperienceLabelId);

        // get all checkboxes that weren't selected
        let checkboxes = $('[id^="work-exp-current"]');

        // hide/show end date and other checkboxes based on checked value
        if (workExperienceCheck.checked === true) {
            workExperienceEndDate.style.display = 'none';
            workExperienceLabel.style.display = 'none';
            workExperienceEndDate.valueAsDate = new Date(1970, 1, 1);

            for (var i = 0; i < checkboxes.length; i++) {
                if (checkboxes[i].checked === false) {
                    checkboxes[i].style.display = 'none'
                }
            }
        }
        else {
            workExperienceLabel.style.display = 'block';
            workExperienceEndDate.style.display = 'block';

            for (var i = 0; i < checkboxes.length; i++) {
                checkboxes[i].style.display = 'block'
            }
        }
    }
}