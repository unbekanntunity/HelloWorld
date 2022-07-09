import React, { Component } from 'react';

class SimpleItem extends Component {
    render() {
        return (
            <p className={`${this.props.className} simple-item`} style={{
                color: this.props.selected ? this.props.selectedTextColor : this.props.unselectedTextColor,
            }} onClick={() => this.props.onClick(this.props.keyProp)}>
                {this.props.name}
            </p>
        )
    }
}

export default SimpleItem;