import React, { Component, createRef } from 'react';

import './DropRightDialog.css';

class DropRightDialog extends Component {
    state = {
        show: this.props.show ?? false
    }


    constructor(props) {
        super(props);

        this.myRef = createRef();
    }

    componentDidMount() {
        document.addEventListener("mousedown", this.handleClickOutside);
    }

    handleClickOutside = (e) => {
        if (!this.myRef.current.contains(e.target)) {
            this.setState({ show: false });
        }
    }

    handleVisibility = () => {
        this.setState({ show: !this.state.show });

        if (!this.state.show) {
            this.props?.onShow();
        }
    }


    render() {
        return (
            <div className="dr-dialog-container" ref={this.myRef}>
                <img src={this.state.show ? this.props.menuOpenedIcon : this.props.menuClosedIcon} alt="" style={{
                    width: this.props.iconSize,
                    height: this.props.iconSize,
                }} onClick={this.handleVisibility}
                />
                <div className={this.state.show ? " dr-dialog-content-show" : "dr-dialog-content"}
                    style={{ zIndex: this.props.zIndex }} >
                    {this.props.children}
                </div>
            </div>
        );
    }
}

export default DropRightDialog;
