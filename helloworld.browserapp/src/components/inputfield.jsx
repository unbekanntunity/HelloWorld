import React, { Component } from 'react';
import './InputField.css';

class InputField extends Component {
    state = {
        edited: false,
        text: this.props.placeholder
    }

    render() {
        return (
            <div className='container'>
                <img className='icon' src={this.props.icon} alt="" />
                <input className='text-edited box'
                    type={this.props.type}
                    placeholder={this.props.placeholder}>
                </input>
            </div>
        );
    }
}

export default InputField;