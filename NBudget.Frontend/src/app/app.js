import ko from 'knockout';
import * as router from './router';

class AppViewModel {
    constructor() {
        this.route = router.currentRoute;
    }
}

var app = new AppViewModel();

export default app;