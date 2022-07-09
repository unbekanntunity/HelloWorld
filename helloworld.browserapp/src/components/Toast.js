import React, { Component } from 'react';
import 'bootstrap/dist/css/bootstrap.css';
import { Toast as ReactToast} from 'react-bootstrap';

class Toast extends Component {
    render() {
        return (
            <ReactToast show={this.props.show} onClose={this.props.onClose}>
                <ReactToast.Header>
                    <img src={this.props.icon} className="rounded me-2" alt="" width="20px" />
                    <strong className="me-auto">{this.props.title}</strong>
                </ReactToast.Header>
                <ReactToast.Body>{this.props.message}</ReactToast.Body>
            </ReactToast>
        );
	}
}

export default Toast;