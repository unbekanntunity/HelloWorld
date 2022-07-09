import React, { Component, createRef } from 'react';
import { RoundButton } from './Button';

import './SpeedDial.css';

class SpeedDial extends Component {
    static Item = RoundButton;

    state = {
        open: false,
        itemClassName: "speedDial-item close",
        toggleButtonIcon: this.props.menuClosedIcon
    }

    constructor(props) {
        super(props);
        this.myRef = createRef();
    }
    
    componentDidMount() {
        document.addEventListener("mousedown", this.handleClickOutside);
    }

    convertToArray = () => {
        const children = Array.from(this.props.children);
        return children.length !== 0 ? children : [this.props.children];
    }

    setToggleButton = (open) => {
        this.setState({
            open: open,
            itemClassName: open ?
                "speedDial-item open" : "speedDial-item close",
            toggleButtonIcon: open ?
                this.props.menuOpenedIcon : this.props.menuClosedIcon
        })
    }

    handleToggleButton = () => {
        this.setToggleButton(!this.state.open);

        if (this.props.onClick) {
            this.props.onClick();
        }
    }

    handleButtonClick = (itemOnClick) => {
        this.setToggleButton(false);
        if (itemOnClick) {
            itemOnClick();
        }
    }

    handleClickOutside = (e) => {
        if (this.myRef.current === null) {
            return;
        }

        if (!this.myRef.current.contains(e.target)) {
            this.setToggleButton(false);
        }
    }

    render() {
        return (
            <div ref={this.myRef}>
                <div className="speedDial-items">
                    {
                        this.props.children &&
                        this.convertToArray().map((item, index) =>
                            <div key={index} className={this.state.itemClassName}>
                                <RoundButton icon={item.props.icon} onClick={() => this.handleButtonClick(item.props.onClick)}
                                    iconSize={this.props.iconSize * this.props.itemFactor} radius={this.props.radius * this.props.itemFactor} />
                            </div>
                        )
                    }
                </div>
                <RoundButton icon={this.state.toggleButtonIcon} radius={this.props.radius} iconSize={this.props.iconSize} onClick={this.handleToggleButton} />
            </div>
        )
    }
}
export default SpeedDial;