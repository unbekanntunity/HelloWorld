import React, { Component } from 'react';

import './UnderlineItem.css';

class UnderlineItem extends Component {
    render() {
        return (
            <p className={`${this.props.className} underline-item`} style={{
                color: this.props.textColor,
                borderBottom: this.props.selected ? `5px solid ${this.props.selectedBorderColor}` : 'none'
            }} onClick={() => this.props.onClick(this.props.keyProp)}>
                {this.props.name}
            </p>
        )
    }
}

export default UnderlineItem;