// require.js looks for the following global when initializing
var require = {
    baseUrl: ".",
    paths: {
        "bootstrap":            "bower_modules/components-bootstrap/js/bootstrap.min",
        "crossroads":           "bower_modules/crossroads/dist/crossroads.min",
        "hasher":               "bower_modules/hasher/dist/js/hasher.min",
        "jquery":               "bower_modules/jquery/dist/jquery",
        "jquery-deparam":       "bower_modules/jquery-deparam/jquery-deparam",
        "moment":               "bower_modules/moment/moment",
        "pikaday":              "bower_modules/pikaday/pikaday",
        "knockout":             "bower_modules/knockout/dist/knockout.debug",
        "knockout-validation":  "bower_modules/knockout-validation/dist/knockout.validation",
        "knockout-projections": "bower_modules/knockout-projections/dist/knockout-projections",
        "signals":              "bower_modules/js-signals/dist/signals.min",
        "text":                 "bower_modules/requirejs-text/text"
    },
    shim: {
        "bootstrap": { deps: ["jquery"] }
    }
};
