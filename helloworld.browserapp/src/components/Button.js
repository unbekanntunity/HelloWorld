import React, { Component } from 'react';

import './Button.css';

export class Button extends Component {
    render() {
        return (
            <button className="button" onClick={this.props.onClick} style={{ width: this.props.width }}>{ this.props.text}</button>
        );
    }
}

export class RoundButton extends Component {
    render() {
        return (
            <button onClick={this.props.onClick} className="round-button" style={{
                width: this.props.radius,
                height: this.props.radius
            }} >
                <img className="round-icon" style={{
                    width: this.props.iconSize ?? '-webkit-fill-available',
                    width: this.props.iconSize ?? '-moz-available'
                }} src={this.props.icon} alt="" />
            </button>
        );
    }
}