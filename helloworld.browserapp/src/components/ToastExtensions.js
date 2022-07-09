import React, { Component } from 'react';
import Toast from '../components/Toast';

import warning from '../images/warning.png';
import notification from '../images/notification.png';

export class ErrorToast extends Component {
	render() {
		return (
			<Toast icon={warning} title="Error" message={this.props.message} show={this.props.show}
				onClose={this.props.onClose} />
		);
	}
}

export class NotificationToast extends Component {
	render() {
		return (
			<Toast icon={notification} title="Notification" message={this.props.message} show={this.props.show}
				onClose={this.props.onClose} />
		);
	}
}
