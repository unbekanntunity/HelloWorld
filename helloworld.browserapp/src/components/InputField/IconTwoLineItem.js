import React, { Component } from 'react';

import './IconTwoLineItem.css';

class IconTwoLineItem extends Component {
    render() {
        return (
            <div className="iconTwoLineItem-container" style={{ padding: `0px ${this.props.paddingY ?? "10px"}` }}>
                <img src={this.props.icon} width={40} height={40} alt="" />
                <div className="iconTwoLineItem-texts">
                    <p className="iconTwoLineItem-header revert-margin" >{this.props.header}</p>
                    <p className="iconTwoLineItem-text revert-margin">{this.props.text}</p>
                </div>
                <p className="iconTwoLineItem-sideNote">{this.props.sideNote}</p>
            </div>
        );
    }
}

export default IconTwoLineItem;