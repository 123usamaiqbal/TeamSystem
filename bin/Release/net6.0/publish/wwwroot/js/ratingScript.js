    document.addEventListener("DOMContentLoaded", function () {
        var form = document.getElementById("ratingForm");

        // Listen for clicks on elements with the "add-score-button" class
        var addScoreButtons = document.querySelectorAll(".add-score-button");
        addScoreButtons.forEach(function (button) {
            button.addEventListener("click", function (event) {
                event.preventDefault(); // Prevent the default link behavior
                var sprintId = button.getAttribute("data-sprintid");
                if (form && sprintId) {
                    // Rest of your existing code to load and display previous ratings
                    var formData = localStorage.getItem("ratingFormData");
                    if (formData) {
                        var data = JSON.parse(formData);
                        for (var i = 0; i < data.length; i++) {
                            var radio = form.querySelector('input[name="viewModelList[' + i + '].SprintRating"][value="' + data[i] + '"]');
                            if (radio) {
                                radio.checked = true;
                            }
                        }
                    }
                }
            });
        });

        // Script to store selected ratings in localStorage before form submission
        form.addEventListener("submit", function () {
            // Your existing code to store selected ratings
        });
    });