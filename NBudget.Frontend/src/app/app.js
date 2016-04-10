import ko from 'knockout';
import * as router from './router';
import auth from './auth'

class AppViewModel {
    constructor() {
        this.route = router.currentRoute;
        this.authenticated = auth.authenticated;
        this.stuff = ko.observable('qwe');
    }
};

var app = new AppViewModel();

export default app;