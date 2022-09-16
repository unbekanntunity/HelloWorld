import React, { Component } from 'react';

import back from '../images/left-arrow2.png';

import MultiInputField from './MultiInputField';
import { Button } from './Button';

import './Dialog.css';
import { sendJSONRequest } from '../requestFuncs';

export class Dialog extends Component {
	render() {
		return (
			<div className="dialog-container">
				<div className="background"/>

				<div className="dialog" style={{
					height: this.props.height,
					width: this.props.width
				}}>
					<div className="dialog-header">
						{
							this.props.backButton &&
							<img style={{ zIndex: '1' }} src={back} height={30} width={30} alt="" onClick={this.props.onBackClick} />
						}
						<div className="title-container">
							<p className="dialog-text" style={{
								fontSize: "18px",
								fontWeight: 'bold'
							}}>{this.props.title}</p>
						</div>
						{
							this.props.rightText && <p className="dialog-text" style={{ color: this.props.rightTextColor }}>{this.props.rightText}</p>
						}
						{
							this.props.rightIcon && <img src={this.props.rightIcon} style={{ zIndex: '1' }} height={30} width={30} alt=""
								onClick={this.props.onRightClick} />
						}
					</div>
					<div className="dialog-body" style={{ padding: this.props.paddingY + " " + this.props.paddingX }}>
						{this.props.children}
					</div>
				</div>
			</div>

		);
	}
}

export class ReportDialog extends Component {
	state = {
		reason: "",
		reportReasons: [
			"Sexual harassment",
			"Spam",
			"Violence"
		],
		showOptions: true,
		showSent: false,
    }

	handleMultiLineChange = (event) => {
	}

	handleReport = () => {
		this.setState({ show: false });
	}

	handleReasonSelected = (item) => {
		if (!item || !item.replace(/\s/g, '').length || item.length === 0) {
			return
        }

		this.setState({
			reason: item,
			showSent: true,
			showOptions: false
		})
    }

	render() {
		return (
			<div className="report-dialog-container">
				{
					this.state.showOptions &&
					<Dialog title="Report" height="fit-content" width="400px" onBackClick={this.props.onClose} backButton={true}>
						{
								this.state.reportReasons.map((item, index) =>
									<div key={index} className="report-item clickable"
									onClick={() => this.handleReasonSelected(item)}>
									<p className="report-item-text">{item}</p>
								</div>
							)}
						<div className="report-item last">
							<p style={{ marginBottom: '10px' }}>Reason not listed</p>
							<MultiInputField placeholder="Enter description" height="100px" maxLetters={10}
								onChange={(event) => this.setState({ reason: event.target.value })} />
							<div className="sendButton">
								<Button text="Send" width="100px" onClick={() => this.handleReasonSelected(this.state.reason)} />
							</div>
						</div>
					</Dialog>
				}
				{
					this.state.showSent &&
					<Dialog title="Thanks for letting us know" height="fit-content" width="400px" paddingX="20px" paddingY="20px">
						<div className="reportDialog-creatorImage-container" >
							<img src={this.props.creatorImage} alt="" width={20} height={20} />
						</div>
						<p className="report-confirmation">Your report successfully sent to our team.<br />Here a other actions u can do against this person.</p>
							<div className="report-otheractions-container">
								<p className="report-otheractions-text"
									onClick={() => this.props.onNotification("You successfully unfollowed this account")}>Unfollow this account</p>
								<p className="report-otheractions-text"
									onClick={() => this.props.onNotification("You successfully blocked this account")}>Block this account</p>
						</div>
						<div className="report-close-container">
							<Button text="Close" onClick={this.props.onClose} />
						</div>
					</Dialog>
				}
			</div>
		)
	}
}


export class UsersDialog extends Component {
	state = {
		users: [],
    }

	componentDidMount() {
		let fullUsers = [];

		for (let i = 0; i < this.props.userIds.length; i++) {
			sendJSONRequest("GET", `/user/get/${this.props.userIds[i]}`, undefined, this.props.tokens.token)
				.then(response => {
					fullUsers = [...fullUsers, response.data]

					if (i === (this.props.userIds.length - 1)) {
						this.setState({ users: fullUsers });
						setTimeout(() => console.log(this.state), 100)
                    }
				}, error => {
					this.props.onError(error.message)
                }
			)
        }
	}


	render() {
		return (
			<Dialog title={this.props.title} width="400px" paddingX="20px" paddingY="20px" onBackClick={this.props.onClose} backButton={true}>
				{
					this.state.users.map((item, index) =>
						<div key={index} className="flex" onClick={() => this.props.onElementClicked(item.id)}>
							<img src={item.imageUrl} alt="" height={40} width={40} />
							<div className="users-dialog-texts">
								<p className="users-dialog-name">{item.userName}</p>
								<p className="users-dialog-email">{item.email}</p>
							</div>
						</div>
					)
				}
				{
					this.state.users.length === 0 &&
					<p>No followers there :( <br /> <br />Maybe you need to upload some content </p>
				}
			</Dialog>
		)
    }
}

export class DeleteConfirmDialog extends Component {
	render() {
		return (
			<Dialog title="Remove permenant?" height="fit-content" width="400px" paddingX="20px" paddingY="20px"
				onBackClick={this.props.onBack} backButton={true}>
				<p>Do you really want to delete this item?</p>
				<div className="center-horizontal">
					<Button margin="0px 5px" text="Cancel" onClick={this.props.onCancel} />
					<Button margin="0px 5px" color="red" text="Remove" onClick={this.props.onConfirm} />
				</div>
			</Dialog>	
		)
    }
}