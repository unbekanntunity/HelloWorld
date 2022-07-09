import React, { Component } from 'react';

import './DropRightDialog.css';

class DropRightDialog extends Component {
    render() {
        return (
            <div className="dr-dialog-container">
                <img src={this.props.show ? this.props.menuOpenedIcon : this.props.menuClosedIcon} alt="" style={{
                    width: this.props.iconSize,
                    height: this.props.iconSize,
                }} onClick={ this.props.onToggleButtonClick}
                />
                <div className={this.props.show ? " dr-dialog-content-show" : "dr-dialog-content"}
                    style={{ zIndex: this.props.zIndex }} >
                    {this.props.children}
                </div>
            </div>
        );
    }
}

export default DropRightDialog;
