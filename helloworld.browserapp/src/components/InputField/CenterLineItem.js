import React, { Component } from 'react';
import './CenterLineItem.css';

class CenterLineItem extends Component {
    render() {
        return (
            <div className="centerLineItem-container" style={{ padding: `0px ${this.props.paddingY ?? "10px"}` }} onClick={this.props.onClick}>
                <p className="centerLineItem-header" style={{ padding: "10px", fontSize: this.props.fontSize }}>{this.props.header}</p>
            </div>
        );
    }
}

export default CenterLineItem;