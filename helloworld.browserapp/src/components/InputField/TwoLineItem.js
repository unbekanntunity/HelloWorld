import React, { Component } from 'react';

import './TwoLineItem.css';

class TwoLineItem extends Component {
    render() {
        return (
            <div className="twoLineItem-container" style={{ padding: `0px ${this.props.paddingY ?? "10px"}` }} onClick={ this.props.onClick}>
                <div className="twoLineItem-texts">
                    <p className="twoLineItem-header" >{this.props.header}</p>
                    <p className="twoLineItem-text">{this.props.text}</p>
                </div>
                <p className="twoLineItem-sideNote">{this.props.sideNote}</p>
                </div>
        );
    }
}

export default TwoLineItem;