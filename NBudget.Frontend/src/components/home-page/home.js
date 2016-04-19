import ko from 'knockout';
import homeTemplate from 'text!./home.html';
import Pikaday from 'pikaday';
import auth from 'app/auth';

class HomeViewModel {
    constructor(route) {
        this.message = ko.computed(() => auth.authenticated() ? 'Welcome back, ' + auth.principal().fname + '.' : 'Welcome to NBudget.' );
        console.log(location.hash);
    }
}

export default { viewModel: HomeViewModel, template: homeTemplate };