import React, { Component, createRef } from 'react';
import { Link } from 'react-router-dom';

import DropDownItem from './DropDownItem';

import './DropDown.css'

class DropDown extends Component {
	static Item = DropDownItem;

	state = {
		dropDownContentClass: "dropdown-content",
		dropDownArrowIcon: this.props.toggleButton.arrowIconClose,
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
			this.setToggleButton(false);
		}
    }

	setToggleButton = (show) => {
		this.setState({
			dropDownContentClass: show ? "dropdown-content-show" : "dropdown-content",
			dropDownArrowIcon: show ? this.props.toggleButton.arrowIconOpen : this.props.toggleButton.arrowIconClose
        })
    }

	handleToggleButton = () => {
		if (this.state.dropDownContentClass === "dropdown-content") {
			this.setState({
				dropDownContentClass: "dropdown-content dropdown-content-show",
				dropDownArrowIcon: this.props.toggleButton.arrowIconOpen
			});
		}
		else {
			this.setState({
				dropDownContentClass: "dropdown-content",
				dropDownArrowIcon: this.props.toggleButton.arrowIconClose
			});
		}

		if (this.props.onHeaderClick !== undefined) {
			this.props.onHeaderClick();
        }
	}

	handleItemClick = (element) => {
		if (element.props.onClick !== undefined) {
			element.props.onClick();
		}

		this.handleToggleButton();
	}

	getItem = (x, index) => {
		if (x.props.linkTo) {
			return (
				<Link key={`link-${index}`} to={x.props.linkTo}>
					<DropDownItem key={index} icon={x.props.icon} iconSize={x.props.iconSize} text={x.props.text}
						onClick={() => this.handleItemClick(x)} textColor={x.props.textColor} lastItem={x === this.props.children[this.props.children.length - 1]} />
				</Link>
			)
		}
		else {
			return (
				<DropDownItem key={index} icon={x.props.icon} iconSize={x.props.iconSize} text={x.props.text}
					onClick={() => this.handleItemClick(x)} textColor={x.props.textColor} lastItem={x === this.props.children[this.props.children.length - 1]} />
			)
		}
	}

	render() {
		const { icon } = this.props.toggleButton;

		return (
			<div className="dropdown" ref={this.myRef} style={{ zIndex: this.props.zIndex }}>
				<div onClick={this.handleToggleButton}>
					{icon !== undefined && <img src={icon} width={this.props.iconSize} height={this.props.iconSize} alt="" />}
					{
						(this.props.toggleButton.arrowIconOpen !== undefined && this.props.toggleButton.arrowIconClose !== undefined) &&
						<img src={this.state.dropDownArrowIcon} width={this.props.arrowIconSize} height={this.props.arrowIconSize}
							alt="" />
					}
				</div>
				<div className={this.state.dropDownContentClass} style={{
					width: this.props.contentWidth ?? 'fit-content',
					left: `-${this.props.contentLeft}`
				}}>
					{this.props.children?.map((x, index) => this.getItem(x, index))}
				</div>
			</div>
		);
	}
}

export default DropDown;

