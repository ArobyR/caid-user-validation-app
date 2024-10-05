$(document).ready(function () {
    $("#loginForm").validate({
        rules: {
            Email: {
                required: true,
                email: true
            },
            Password: {
                required: true,
                minlength: 6
            }
        },
        messages: {
            Email: {
                required: "Please enter your email address",
                email: "Please enter a valid email address"
            },
            Password: {
                required: "Please provide a password",
                minlength: "Your password must be at least 6 characters long"
            }
        },
        errorClass: "text-danger",
        submitHandler: function (form) {
            form.submit();
        }
    });
});
