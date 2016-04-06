import ko from 'knockout';
import homeTemplate from 'text!./home.html';
import Pikaday from 'pikaday';

class HomeViewModel {
    constructor(route) {
        this.message = ko.observable('Welcome to NBudget.');
    }
}

export default { viewModel: HomeViewModel, template: homeTemplate };